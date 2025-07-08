using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Application.DTOs;
using System.Threading.Tasks;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TweetLikeController : ControllerBase
    {
        private readonly ITweetLikeService _tweetLikeService;

        public TweetLikeController(ITweetLikeService tweetLikeService)
        {
            _tweetLikeService = tweetLikeService;
        }

        [HttpPost("like")]
        public async Task<IActionResult> LikeTweet([FromBody] LikeTweetRequest request)
        {
            var result = await _tweetLikeService.LikeTweetAsync(request.UserId, request.TweetId);
            if (result)
                return Ok(new { message = "Tweet liked." });
            return BadRequest(new { message = "Already liked." });
        }
        

        [HttpPost("unlike")]
        public async Task<IActionResult> UnlikeTweet(int userId, int tweetId)
        {
            var result = await _tweetLikeService.UnlikeTweetAsync(userId, tweetId);
            if (result)
                return Ok(new { message = "Tweet unliked." });
            return NotFound(new { message = "Like not found." });
        }

        [HttpGet("tweet/{tweetId}/likes")]
        public async Task<IActionResult> GetLikesByTweet(int tweetId)
        {
            var likes = await _tweetLikeService.GetLikesByTweetIdAsync(tweetId);
            return Ok(likes);
        }

        [HttpGet("user/{userId}/likes")]
        public async Task<IActionResult> GetLikesByUser(int userId)
        {
            var likes = await _tweetLikeService.GetLikesByUserIdAsync(userId);
            return Ok(likes);
        }

        [HttpGet("is-liked")]
        public async Task<IActionResult> IsTweetLiked(int userId, int tweetId)
        {
            var isLiked = await _tweetLikeService.IsTweetLikedAsync(userId, tweetId);
            return Ok(new { isLiked });
        }

        [HttpGet("tweet/{tweetId}/count")]
        public async Task<IActionResult> GetLikesCount(int tweetId)
        {
            var count = await _tweetLikeService.GetLikesCountAsync(tweetId);
            return Ok(new { count });
        }
    }
}
