using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.Services.interfaces;
using System.Security.Claims;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimelineController : ControllerBase
    {
        private readonly ITimelineService _timelineService;

        public TimelineController(ITimelineService timelineService)
        {
            _timelineService = timelineService;
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHomeTimelineAsync(
            int userId,
            int limit = 20,
            DateTime? cursor = null)
        {
            try
            {
                var tweets = await _timelineService.GetHomeTimelineAsync(userId, limit, cursor);
                return Ok(tweets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserTimelineAsync(
            string username,
            int limit = 20,
            DateTime? cursor = null)
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
