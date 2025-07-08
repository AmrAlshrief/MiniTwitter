using System;

namespace MiniTwitter.Core.Domain.Events;

public record UserFollowedEvent(int FollowerId, int FollowedId) : IDomainEvent;