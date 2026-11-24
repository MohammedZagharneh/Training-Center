using DataAccess.Connections;
using DataAccess.Entities;
using Domain.DTOs.RefreshToken;
using Domain.DTOs.User;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace DataAccess.Repositories
{
    public class AuthRepository
    {
        private readonly SqlConnectionFactory _factory;
        private readonly TrainingCenterContext _context;

        public AuthRepository(SqlConnectionFactory Factory, TrainingCenterContext context)
        {
            _factory = Factory;
            _context = context;
        }

        public async Task<IEnumerable<UsersListDto>> GetUsersAsync()
        {
            List<UsersListDto> list = new List<UsersListDto>();

            using (SqlConnection conn = _factory.CreateConnection())
            using(SqlCommand cmd = new SqlCommand("SP_GetUsersList", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    int UserIdIndex = reader.GetOrdinal("UserID");
                    int UsernameIndex = reader.GetOrdinal("Username");
                    int RoleIndex = reader.GetOrdinal("Role");
                    int IsActiveIndex = reader.GetOrdinal("IsActive");
                    int PersonIdIndex = reader.GetOrdinal("PersonID");
                    int FirstNameIndex = reader.GetOrdinal("FirstName");
                    int LastNameIndex = reader.GetOrdinal("LastName");
                    int NationalNumberIndex = reader.GetOrdinal("NationalNumber");
                    int EmailIndex = reader.GetOrdinal("Email");
                    int PhoneNumberIndex = reader.GetOrdinal("PhoneNumber");

                    while (await reader.ReadAsync())
                    {

                        list.Add(new UsersListDto
                        {
                            UserId = reader.GetInt32(UserIdIndex),
                            Username = reader.GetString(UsernameIndex),
                            Role = reader.GetString(RoleIndex),
                            IsActive = reader.GetBoolean(IsActiveIndex),
                            PersonId = reader.GetInt32(PersonIdIndex),
                            NationalNumber = reader.GetString(NationalNumberIndex),
                            FirstName = reader.GetString(FirstNameIndex),
                            LastName = reader.GetString(LastNameIndex),
                            Email = reader.IsDBNull(EmailIndex) ? null : reader.GetString(EmailIndex),
                            PhoneNumber = reader.IsDBNull(PhoneNumberIndex) ? null : reader.GetString(PhoneNumberIndex)
                        });
                    }
                }
    
            }

            return list;

        }

        public async Task<UserDetailsDto?> GetUserDetails(string Username)
        {
            UserDetailsDto? user = null;

            using (SqlConnection conn = _factory.CreateConnection())
            using (SqlCommand cmd = new SqlCommand("SP_GetUserDetails", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", Username);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {

                        user = new UserDetailsDto
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            Role = reader.GetString(reader.GetOrdinal("Role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            PersonId = reader.GetInt32(reader.GetOrdinal("PersonID"))
                        };
                    }

                    return user;
                }
            }
        }

        public async Task<int?> RegisterAsync(UserAddDto user)
        {
            var connection = (SqlConnection)_context.Database.GetDbConnection();

            var transaction = (SqlTransaction)_context.Database.CurrentTransaction!.GetDbTransaction();

            using (SqlCommand command = new SqlCommand("SP_Register", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Transaction = transaction;

                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@role", user.Role.ToString());
                command.Parameters.AddWithValue("@isActive", user.IsActive);
                command.Parameters.AddWithValue("@personID", user.PersonId);

                try
                {
                    object? result = await command.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
                catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                {
                    return null;
                }
            }
        }

        public async Task<RefreshTokenDetailsDto?> GetRefreshTokenDetailsAsync(string refreshTokenHash)
        {
            RefreshTokenDetailsDto? refreshToken = null;

            using (SqlConnection conn = _factory.CreateConnection())
            using (SqlCommand cmd = new SqlCommand("SP_GetRefreshTokenDetails", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@refreshTokenHash", refreshTokenHash);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        refreshToken = new RefreshTokenDetailsDto
                        {
                            RefreshTokenId = reader.GetInt32(reader.GetOrdinal("RefreshTokenID")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                            ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                            RevokedAt = reader.GetDateTime(reader.GetOrdinal("RevokedAt")),
                            Role = reader.GetString(reader.GetOrdinal("Role")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        };
                    }

                    return refreshToken;
                }
            }
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshTokenHash)
        {
            using(SqlConnection conn = _factory.CreateConnection())
            using (SqlCommand cmd = new SqlCommand("SP_RevokeRefreshToken", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@refreshTokenHash", refreshTokenHash);

                object? result = await cmd.ExecuteScalarAsync();

                return Convert.ToBoolean(result);
            }
        }

        public async Task<int> AddRefreshTokenAsync(RefreshTokenCreateDto refreshToken)
        {
            using (SqlConnection conn = _factory.CreateConnection())
            using (SqlCommand cmd = new SqlCommand("SP_AddRefreshToken", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@refreshTokenHash", refreshToken.RefreshTokenHash);
                cmd.Parameters.AddWithValue("@expiresAt", refreshToken.ExpiresAt);
                cmd.Parameters.AddWithValue("@userID", refreshToken.UserId);

                object? result = await cmd.ExecuteScalarAsync();

                return Convert.ToInt32(result); // returns zero if not added
            }
        }

        public async Task<bool> ChangePasswordAsync(int UserId, string PasswordHash)
        {
            using (SqlConnection conn = _factory.CreateConnection())
            using (SqlCommand cmd = new SqlCommand("SP_ChangePassword", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userID", UserId);
                cmd.Parameters.AddWithValue("@newPasswordHash", PasswordHash);

                object? result = await cmd.ExecuteScalarAsync();

                return Convert.ToBoolean(result);
            }
        }
    }
}
