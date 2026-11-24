using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess.TransactionManager;
using Domain.DTOs.Instructor;
using Domain.DTOs.User;
using Domain.Enums;
using Domain.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Security.Cryptography;
using Domain.DTOs.RefreshToken;

namespace Business.Services
{
    public class AuthService
    {
        private readonly AuthRepository _authRepo;
        private readonly PersonRepository _personRepo;
        private readonly EfTransactionManager _transactionManager;
        private readonly IConfiguration _config;

        public AuthService(AuthRepository authRepo, PersonRepository personRepo, EfTransactionManager transactionManager, IConfiguration config)
        {
            _authRepo = authRepo;
            _personRepo = personRepo;
            _transactionManager = transactionManager;
            _config = config;
        }

        private string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private string HashRefreshToken(string refreshToken)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

            return Convert.ToHexString(bytes);
        }

        private async Task<(string JwtToken, string RefreshToken)> PrepareTokens(int userId, string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };


            string? keyString = _config["Jwt:Key"];

            if (keyString == null)
                throw new InvalidOperationException("JWT signing key is not configured!");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "TrainingCenterAPI",
                audience: "TrainingCenterAPIUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            //Preparing Tokens
            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            string refreshToken = GenerateRefreshToken();

            //Saving Refresh Token in DB
            string refreshTokenHash = HashRefreshToken(refreshToken);
            DateTime expiresAt = DateTime.UtcNow.AddDays(7);
            int? savingRefreshTokenResult = await _authRepo.AddRefreshTokenAsync(refreshTokenHash, expiresAt, userId);

            //only a non logical error can return null here, all logical paths and edge cases are covered
            if (savingRefreshTokenResult == null)
                throw new InvalidOperationException("something went wrong!");

            return (accessToken, refreshToken);
        }

        public async Task<IEnumerable<UsersListDto>> GetUsersAsync()
        {
            return await _authRepo.GetUsersAsync();
        }

        public async Task<RegisterResult> RegisterAsync(UserRegisterDto user)
        {
            await _transactionManager.BeginTransactionAsync();

            try
            {
                Person newPerson = new Person
                {
                    NationalNumber = user.Person.NationalNumber,
                    FirstName = user.Person.FirstName,
                    LastName = user.Person.LastName,
                    Email = user.Person.Email,
                    BirthDate = user.Person.BirthDate,
                    PhoneNumber = user.Person.PhoneNumber

                };

                bool isAdded = await _personRepo.AddAsync(newPerson);

                if (!isAdded)
                {
                    await _transactionManager.RollbackAsync();
                    return new RegisterResult(RegisterStatus.DuplicatePersonData, null);
                }


                string username = user.Username;
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
                bool isActive = true;
                RoleStatus role = RoleStatus.user;
                int personId = newPerson.PersonId;


                int? newUserId = await _authRepo.RegisterAsync(username, passwordHash, role, isActive, personId);

                if (newUserId == null)
                {
                    await _transactionManager.RollbackAsync();
                    return new RegisterResult(RegisterStatus.DuplicateUsername, null);
                }

                await _transactionManager.CommitAsync();

                return new RegisterResult(RegisterStatus.Success, newUserId);
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                throw;
            }
        }

        public async Task<LoginRequestResult> LoginAsync(LoginRequestDto login)
        {

            UserDetailsDto? user = await _authRepo.GetUserDetailsAsync(login.Username);


            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                return new LoginRequestResult(LoginRequestStatus.Unauthorized, null);


            (string JwtToken, string RefreshToken) TokensToSend = await PrepareTokens(user.UserId, user.Username, user.Role);

            LoginResponseDto response = new LoginResponseDto { JwtToken = TokensToSend.JwtToken, RefreshToken = TokensToSend.RefreshToken};

            return new LoginRequestResult(LoginRequestStatus.Success, response);
        }

        public async Task<RefreshTokenResult> RefreshTokenAsync(string RefreshToken)
        {

            string RefreshTokenHash = HashRefreshToken(RefreshToken);

            RefreshTokenDetailsDto? refreshToken = await _authRepo.GetRefreshTokenDetailsAsync(RefreshTokenHash);

            //RevokedAt != null is a security event (I will audit it later)
            if (refreshToken == null || refreshToken.RevokedAt != null || refreshToken.ExpiresAt <= DateTime.UtcNow || !refreshToken.IsActiveUser)
                return new RefreshTokenResult(RefreshTokenStatus.Failed, null);


            bool RevokeResult = await _authRepo.RevokeRefreshTokenAsync(RefreshTokenHash);

            //only a non logical error can return false here
            //because I already made sure that refresh token exists and is not already revoked
            if (!RevokeResult)
                throw new InvalidOperationException("something went wrong!");


            (string JwtToken, string RefreshToken) TokensToSend = await PrepareTokens(refreshToken.UserId, refreshToken.Username, refreshToken.UserRole);

            return new RefreshTokenResult(RefreshTokenStatus.Success, 
                new RefreshResponseDto { NewJwtToken = TokensToSend.JwtToken, RefreshToken = TokensToSend.RefreshToken });
        }

        public async Task LogoutAsync(string RefreshToken)
        {
            string RefreshTokenHash = HashRefreshToken(RefreshToken);

            //doesn't matter if user exists or active or refresh token exists or revoked or expired
            //for security we will return only Ok response, that is why I didn't use boolean value returned here
            await _authRepo.RevokeRefreshTokenAsync(RefreshTokenHash); 
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequestDto changePassword, string Username)
        {
            UserDetailsDto? user = await _authRepo.GetUserDetailsAsync(Username);

            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(changePassword.OldPassword, user.PasswordHash))
                return false;

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);

            bool ChangePasswordResult = await _authRepo.ChangePasswordAsync(user.UserId, newPasswordHash);

            if(!ChangePasswordResult)
                throw new InvalidOperationException("something went wrong!");

            await _authRepo.LogoutFromAllDevicesAsync(user.UserId);

            return true;
        }

        public async Task<ChangeActivityResult> ChangeUserActivityAsync(int UserId, int AdminUserId, bool newActivity)
        {
            if (UserId == AdminUserId)
                return ChangeActivityResult.FailChangeSelfActivity;

            User? user = await _authRepo.GetUserEntityAsync(UserId);

            if (user == null)
                return ChangeActivityResult.UserNotFound;

            if (user.IsActive == newActivity)
                return ChangeActivityResult.UserAlreadyInRequestedState;

            await _transactionManager.BeginTransactionAsync();
            try
            {
                user.IsActive = newActivity;

                await _authRepo.SaveUpdatesAsync();

                if (!newActivity)
                    await _authRepo.LogoutFromAllDevicesSharedConnectionAsync(UserId);

                await _transactionManager.CommitAsync();

                return ChangeActivityResult.Success;
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                throw;
            }

            

        }

        public async Task<ChangeRoleResult> ChangeUserRoleAsync(int UserId, int AdminUserId, RoleStatus newRole)
        {
            if (UserId == AdminUserId)
                return ChangeRoleResult.FailChangeSelfRole;

            User? user = await _authRepo.GetUserEntityAsync(UserId);

            if (user == null)
                return ChangeRoleResult.UserNotFound;

            if (user.Role == newRole.ToString())
                return ChangeRoleResult.UserAlreadyHasRole;

            user.Role = newRole.ToString();

            await _authRepo.SaveUpdatesAsync();

            return ChangeRoleResult.Success;
        }
    }
}