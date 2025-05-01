using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }  // Primary Key
        public string Content { get; set; } = string.Empty;  // Comment text
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Time of comment creation
        public bool IsDeleted { get; set; } = false;  // Flag for soft delete

        // Foreign Key for User
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int TweetId { get; set; }
        public Tweet Tweet { get; set; } = null!;

        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
