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

    public enum ChangeActivityResult
    {
        Success, FailChangeSelfActivity, UserNotFound, UserAlreadyInRequestedState
    }

    public enum ChangeRoleResult
    {
        Success, FailChangeSelfRole, UserNotFound, UserAlreadyHasRole
    }

    public record LoginRequestResult(LoginRequestStatus Status, LoginResponseDto? Tokens);


    public record RegisterResult(RegisterStatus Status, int? UserId);

}
