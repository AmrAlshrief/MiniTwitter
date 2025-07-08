using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using System.Security.Claims;
using MiniTwitter.Core.Domain.Entities;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TweetController : ControllerBase
    {
        private readonly ITweetService _tweetService;

        public TweetController(ITweetService tweetService)
        {
            _tweetService = tweetService;
        }
        [Authorize]
        [HttpPost("tweet")]
        public async Task<IActionResult> AddTweet([FromBody] CreateTweetRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var tweet = await _tweetService.AddTweetAsync(request.Content, userId);


            var response = new TweetResponse
            {
                Id = tweet.Id,
                Content = tweet.Content,
                CreatedAt = tweet.CreatedAt,
                UserId = tweet.UserId
            };

            return Ok(response);
        }

        [HttpGet("tweet")]
        [Authorize]
        public async Task<IActionResult> GetUserTweets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            Console.WriteLine($"UserId: {userId}");
            var tweets = await _tweetService.GetTweetsByUserIdAsync(userId);

            var response = tweets.Select(t => new TweetResponse
            {
                Id = t.Id,
                Content = t.Content,
                CreatedAt = t.CreatedAt,
                UserId = t.UserId
            });
            if (response == null || !response.Any())
                return NotFound(new { message = "No tweets found." });      
            return Ok(response);

        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTweet(int id, [FromBody] CreateTweetRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            //if (userId == null)
            //    return Unauthorized();

            try
            {
                var updatedTweet = await _tweetService.UpdateTweetAsync(id, request.Content, userId);

                return Ok(new TweetResponse
                {
                    Id = updatedTweet.Id,
                    Content = updatedTweet.Content,
                    CreatedAt = updatedTweet.CreatedAt,
                    UserId = updatedTweet.UserId
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTweet(int id) 
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            Console.WriteLine(id);
            try
            {
                await _tweetService.DeleteTweetAsync(id, userId);
                return Ok(new { message = "Tweet deleted." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }

   

}
