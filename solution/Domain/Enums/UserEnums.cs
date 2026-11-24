using Domain.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum RoleStatus
    {
        admin, user
    }


    public enum RegisterStatus
    {
        Success, DuplicateUsername, DuplicatePersonData
    }

    public enum LoginRequestStatus
    {
        Success, Unauthorized
    }

    public record LoginRequestResult(LoginRequestStatus Status, string? JwtToken);


    public record RegisterResult(RegisterStatus Status, int? UserId);

}
