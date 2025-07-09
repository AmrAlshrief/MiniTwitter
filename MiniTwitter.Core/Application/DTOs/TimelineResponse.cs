using System;

namespace MiniTwitter.Core.Application.DTOs;

public class TimelineResponse
{
    public IEnumerable<TweetDto> Tweets { get; set; }
    public string NextCursor { get; set; }
    public bool HasMore { get; set; }
}
