namespace MiniTwitter.Core.Domain.Events
{
    public record TweetRetweetedEvent(int RetweeterId, int TweetId) : IDomainEvent;
}
