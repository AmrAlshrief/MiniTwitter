using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.Services.interfaces;
using System.Security.Claims;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }
        [HttpPost("{followingId}")]
        [Authorize]
        public async Task<IActionResult> FollowUser(int followingId) 
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID not found");

            int followerId = int.Parse(userIdClaim.Value);

            try 
            {
                await _followService.FollowUserAsync(followerId, followingId);
                return Ok(new { message = "Followed successfully. " });
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
            catch(Exception ex) 
            {
                return StatusCode(500, new { message = "An error occured.", detail = ex.Message });
            }
        }

        [HttpDelete("{followingId}")]
        [Authorize]
        public async Task<IActionResult> UnFollowUser(int followingId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID not found");

            int followerId = int.Parse(userIdClaim.Value);

            try
            {
                await _followService.UnfollowUserAsync(followerId, followingId);
                return Ok(new { message = "Unfollowed successfully. " });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occured.", detail = ex.Message });
            }
        }

        [HttpGet("followers/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetFollowers(int userId) 
        {
            try 
            {
                var followers = await _followService.GetFollowersAsync(userId);
                return Ok(followers);
            }
            catch(Exception ex) 
            {
                return StatusCode(500, new { message = "An error occured.", detail = ex.Message });
            }
        }

        [HttpGet("following/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetFollowing(int userId)
        {
            try
            {
                var following = await _followService.GetFollowingAsync(userId);
                return Ok(following);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occured.", detail = ex.Message });
            }
        }
    }
}
