namespace MiniTwitter.Core.Domain.Events
{
    public record TweetLikedEvent(int LikerId, int TweetId) : IDomainEvent;
}
