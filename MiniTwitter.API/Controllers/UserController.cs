using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Infrastructure.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public UserController(IUserService userService, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try 
            {
                var result = await _userService.RegisterUserAsync(registerDto);
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
                var user = await _userService.LoginUserAsync(login);
                if (user == null)
                    return Unauthorized("User not authorized");
                var token = _jwtTokenGenerator.GenerateToken(user);
                

                return Ok(new {token});
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something wrong");
                return Unauthorized(ex.Message);
            }
           
        }

        [Authorize]
        [HttpPost("profile-image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var updatedUser = await _userService.UpdateProfileImageAsync(userId, file);
            return Ok(updatedUser);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var updated = await _userService.UpdateUserAsync(userId, dto);
            return Ok(updated);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Try to get user id from claims (adjust claim type as per your JWT)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type.EndsWith("nameidentifier"));
            if (userIdClaim == null)
                return Unauthorized();

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return Unauthorized();

            var user = await _userService.IsUserActiveAsync(userId);
            if (user == null)
                return NotFound();

            var result = new UserWithRoleDto
            {
                Id = user.Id,
                UserName = user.Username,
                Email = user.Email,
                Role = user.Role,
                ProfileImage = user.ProfilePictureUrl
            };
            Console.WriteLine(result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> getUsers()
        {
            try 
            {
                var user = await _userService.GetAllAsync();
                if (user == null)
                    return NotFound();
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
           
        }
    }
    
}
