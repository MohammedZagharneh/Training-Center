using Business.Services;
using Domain.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Domain.DTOs.RefreshToken;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;

namespace APIs.Controllers
{
    [Authorize]
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }




        [Authorize(Roles = "admin")]
        [HttpGet("all-users" , Name = "GetAllUsers")]
        [ProducesResponseType(typeof(IEnumerable<UsersListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersAsync()
        {
            return Ok( await _authService.GetUsersAsync());
        }




        [AllowAnonymous]
        [HttpPost("register", Name = "Register")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [EnableRateLimiting("RegisterLimiter")]
        public async Task<IActionResult> RegisterAsync(UserRegisterDto user)
        {
            RegisterResult result = await _authService.RegisterAsync(user);

            switch (result.Status)
            {
                case RegisterStatus.DuplicatePersonData:
                    return Conflict("National number, phone number and email should be unique!");

                case RegisterStatus.DuplicateUsername:
                    return Conflict("Username should be unique!");

                case RegisterStatus.Success:
                    return StatusCode(StatusCodes.Status201Created, result.UserId);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




        [AllowAnonymous]
        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("LoginLimiter")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginRequest)
        {
            LoginRequestResult result = await _authService.LoginAsync(loginRequest);

            switch(result.Status)
            {
                case LoginRequestStatus.Unauthorized:
                    return Unauthorized("Invalid credentials");

                case LoginRequestStatus.Success:
                    return Ok(result.Tokens!);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




        [AllowAnonymous]
        [HttpPost("refresh-token", Name = "RefreshToken")]
        [ProducesResponseType(typeof(RefreshResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("RefreshLimiter")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshRequestDto refersh)
        {
            RefreshTokenResult result = await _authService.RefreshTokenAsync(refersh.RefreshToken);

            switch(result.Status)
            {
                case RefreshTokenStatus.Failed:
                    return Unauthorized("Invalid refresh request data!");

                case RefreshTokenStatus.Success:
                    return Ok(result.Tokens!);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




        [HttpPost("logout", Name = "Logout")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> LogoutAsync(LogoutRequestDto logoutRequest)
        {
            await _authService.LogoutAsync(logoutRequest.RefreshToken);

            return Ok("Logged out successfully");
        }




        [HttpPut("change-password", Name = "ChangePassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequestDto changePassword)
        {
            string username = User.FindFirstValue(ClaimTypes.Name)!;

            bool result = await _authService.ChangePasswordAsync(changePassword, username);

            if(result)
                return NoContent();

            return Unauthorized("Invalid credentials");
        }




        [Authorize(Roles = "admin")]
        [HttpPut("activate-user", Name = "ActivateUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateUserAsync(UserChangeActivityDto changeActivity)
        {
            int AdminUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ChangeActivityResult result = await _authService.ChangeUserActivityAsync(changeActivity.UserId, AdminUserId, true);

            switch(result)
            {
                case ChangeActivityResult.FailChangeSelfActivity:
                    return Conflict("You can't change your activity!");

                case ChangeActivityResult.UserNotFound:
                    return NotFound($"User with ID = {changeActivity.UserId} was not found!");

                case ChangeActivityResult.UserAlreadyInRequestedState:
                    return Conflict("User already active!");

                case ChangeActivityResult.Success:
                    return NoContent();

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




        [Authorize(Roles = "admin")]
        [HttpPut("deactivate-user", Name = "DeactivateUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateUserAsync(UserChangeActivityDto changeActivity)
        {
            int AdminUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ChangeActivityResult result = await _authService.ChangeUserActivityAsync(changeActivity.UserId, AdminUserId, false);

            switch (result)
            {
                case ChangeActivityResult.FailChangeSelfActivity:
                    return Conflict("You can't change your activity!");

                case ChangeActivityResult.UserNotFound:
                    return NotFound($"User with ID = {changeActivity.UserId} was not found!");

                case ChangeActivityResult.UserAlreadyInRequestedState:
                    return Conflict("User already inactive!");

                case ChangeActivityResult.Success:
                    return NoContent();

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [Authorize(Roles = "admin")]
        [HttpPut("change-user-role", Name = "ChangeUserRole")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangeUserRoleAsync(UserChangeRoleDto changeRole)
        {
            int AdminUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            ChangeRoleResult result = await _authService.ChangeUserRoleAsync(changeRole.UserId, AdminUserId, changeRole.Role);

            switch (result)
            {
                case ChangeRoleResult.FailChangeSelfRole:
                    return Conflict("You can't change your own role!");

                case ChangeRoleResult.UserNotFound:
                    return NotFound($"User with ID = {changeRole.UserId} was not found!");

                case ChangeRoleResult.UserAlreadyHasRole:
                    return Conflict("User already has this role!");

                case ChangeRoleResult.Success:
                    return NoContent();

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
