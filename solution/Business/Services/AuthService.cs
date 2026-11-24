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

                UserAddDto newUser = new UserAddDto
                {
                    Username = user.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    IsActive = true,
                    Role = RoleStatus.user,
                    PersonId = newPerson.PersonId
                };


                int? newUserId = await _authRepo.RegisterAsync(newUser);

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

            UserDetailsDto? user = await _authRepo.GetUserDetails(login.Username);


            if (user == null)
                return new LoginRequestResult(LoginRequestStatus.Unauthorized, null);

            if(!user.IsActive)
                return new LoginRequestResult(LoginRequestStatus.Unauthorized, null);


            bool isValidPassword = BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash);


            if (!isValidPassword)
                return new LoginRequestResult(LoginRequestStatus.Unauthorized, null);


            var claims = new[]
            {             
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),            
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
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

            return new LoginRequestResult(LoginRequestStatus.Success, new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
