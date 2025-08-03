using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
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
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
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
                Name = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                Bio = user.Bio,
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
        [Authorize]
        [HttpGet("profile/{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            try
            {
                var userProfile = await _userService.GetUserProfileAsync(username);
                if (userProfile == null)
                    return NotFound(new { message = "User not found" });

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                var result = new UserWithRoleDto
                {
                    Id = user.Id,
                    UserName = user.Username,
                    Name = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email,
                    Bio = user.Bio,
                    Role = user.Role,
                    ProfileImage = user.ProfilePictureUrl
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        

    }
    
}
