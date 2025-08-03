using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.Core.Application.DTOs
{
    public class CreateCommentDto
    {
        public int UserId { get; set; }
        public int TweetId { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;
    }
}