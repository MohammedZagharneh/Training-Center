using Business.Services;
using Domain.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginRequest)
        {
            LoginRequestResult result = await _authService.LoginAsync(loginRequest);

            switch(result.Status)
            {
                case LoginRequestStatus.Unauthorized:
                    return Unauthorized("Invalid credentials");

                case LoginRequestStatus.Success:
                    return Ok(new LoginResponseDto { JwtToken = result.JwtToken! });

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
