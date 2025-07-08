using System;

namespace MiniTwitter.Core.Domain.Entities;

public class CommentLike
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CommentId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Comment Comment { get; set; }
}
