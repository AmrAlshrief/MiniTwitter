using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Infrastructure.Authentication;
using System;
using System.Threading.Tasks;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthController(IAuthService authService, IJwtTokenGenerator jwtTokenGenerator)
        {
            _authService = authService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterUserAsync(registerDto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            try
            {
                var user = await _authService.LoginUserAsync(login);
                if (user == null)
                    return Unauthorized("User not authorized");

                var token = _jwtTokenGenerator.GenerateToken(user);
                return Ok(new { token = token, user = new { user.Id, user.Username, user.Email, user.ProfilePictureUrl } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action(nameof(HandleGoogleResponse), "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return BadRequest("Missing or invalid Authorization header");

            string token = authHeader.Substring("Bearer ".Length).Trim();
            await _authService.LogoutAsync(token);

            return Ok(new { message = "Logout successful" });
        }

    }
}