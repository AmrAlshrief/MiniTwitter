using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Infrastructure.Authentication;

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
                    return Unauthorized();
                var token = _jwtTokenGenerator.GenerateToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
           
        }

        [HttpGet("users")]
        public async Task<IActionResult> getUsers(LoginDto login)
        {
            try 
            {
                var user = await _userService.GetAllAsync();
                if (user == null)
                    return Unauthorized();
                
                return Ok( user);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
           
        }
    }
    
}
