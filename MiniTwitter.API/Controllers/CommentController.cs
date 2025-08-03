using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniTwitter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("User ID claim missing or invalid");
            }

            createCommentDto.UserId = userId;

            var commentDto = await _commentService.CreateCommentAsync(createCommentDto);
            return Ok(commentDto);
        }

        [HttpGet("{tweetId}")]
        [Authorize]
        public async Task<IActionResult> GetCommentsByTweetId(int tweetId)
        {
            var commentsDto = await _commentService.GetCommentsByTweetIdAsync(tweetId);
            return Ok(commentsDto);
        }
        
        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("User ID claim missing or invalid");
            }

            // TODO: Add authorization check to ensure the user is allowed to delete this comment
            // This would typically involve checking if the user is the comment owner or has admin rights

            await _commentService.DeleteCommentAsync(commentId);
            return Ok(new { message = "Comment deleted successfully" });
        }
        
        [HttpPut("{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int commentId, UpdateCommentDto updateCommentDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("User ID claim missing or invalid");
            }

            // TODO: Add authorization check to ensure the user is allowed to update this comment
            // This would typically involve checking if the user is the comment owner

            var commentDto = await _commentService.UpdateCommentAsync(commentId, updateCommentDto);
            return Ok(commentDto);
        }
    }
}