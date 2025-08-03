using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Infrastructure.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheTestController : ControllerBase
    {
        private readonly ICacheService _cache;

        public CacheTestController(ICacheService cache)
        {
            _cache = cache;
        }

        [HttpGet("set")]
        public async Task<IActionResult> Set()
        {
            await _cache.SetAsync("mykey", "Hello from Redis");
            return Ok("Value set");
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var value = await _cache.GetAsync<string>("mykey");
            return Ok(value ?? "No value");
        }

        [HttpGet("test-blacklist/{jti}")]
        public async Task<IActionResult> TestBlacklist(string jti)
        {
            // Add to blacklist
            await _cache.SetAsync($"blacklist:{jti}", "1", TimeSpan.FromMinutes(30));
            return Ok($"Added {jti} to blacklist");
        }

        [HttpGet("check-blacklist/{jti}")]
        public async Task<IActionResult> CheckBlacklist(string jti)
        {
            var exists = await _cache.ExistsAsync($"blacklist:{jti}");
            return Ok(new { jti = jti, isBlacklisted = exists });
        }

        [HttpGet("get-jti")]
        [Authorize]
        public IActionResult GetJti()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest("No valid Authorization header found");
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrEmpty(jti))
                {
                    return BadRequest("JTI not found in token");
                }

                return Ok(new { jti = jti, token = token });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error extracting JTI: {ex.Message}");
            }
        }
    }
}