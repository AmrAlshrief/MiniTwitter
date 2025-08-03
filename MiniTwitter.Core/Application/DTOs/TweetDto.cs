using System;

namespace MiniTwitter.Core.Application.DTOs
{
    public class TweetDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; } 
        public bool IsLikedByCurrentUser { get; set; }
        public UserPreviewDto User { get; set; }
    }

}

