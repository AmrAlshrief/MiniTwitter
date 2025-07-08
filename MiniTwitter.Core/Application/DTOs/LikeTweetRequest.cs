using System;

namespace MiniTwitter.Core.Application.DTOs;

public class LikeTweetRequest
{
    public int UserId { get; set; }
    public int TweetId { get; set; }
}
