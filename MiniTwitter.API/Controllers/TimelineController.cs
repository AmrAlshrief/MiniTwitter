using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniTwitter.Core.Application.Services.interfaces;
using System.Security.Claims;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimelineController : ControllerBase
    {
        private readonly ITimelineService _timelineService;
        private readonly ILogger<TimelineController> _logger;

        public TimelineController(
            ITimelineService timelineService,
            ILogger<TimelineController> logger)
        {
            _timelineService = timelineService ?? throw new ArgumentNullException(nameof(timelineService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _logger.LogInformation("TimelineController initialized");
        }

        [HttpGet("home")]
        [Authorize]
        public async Task<IActionResult> GetHomeTimelineAsync(
            int limit = 20,
            string? cursor = null)
        {
            //_logger.LogInformation("GetHomeTimelineAsync called with userId: {UserId}, limit: {Limit}, cursor: {Cursor}", 
               // userId, limit, cursor ?? "null");
                
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("User ID not found");
                if (!int.TryParse(userIdClaim.Value, out int userId))
                    return BadRequest("Invalid User ID");

                var tweets = await _timelineService.GetHomeTimelineAsync(userId, limit, cursor);
                //_logger.LogInformation("Successfully retrieved {Count} tweets", tweets?.Tweets?.Count ?? 0);
                return Ok(tweets);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in GetHomeTimelineAsync for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("user/{username}")]
        [Authorize]
        public async Task<IActionResult> GetUserTimelineAsync(
            string username,
            int limit = 20,
            string? cursor = null)
        {
            try
            {
                var tweets = await _timelineService.GetUserTimelineAsync(username, limit, cursor);
                return Ok(tweets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
            }
        }
    
    }
}
