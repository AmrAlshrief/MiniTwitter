using System.Threading.Tasks;
using MiniTwitter.Core.Application.Events;
using MiniTwitter.Core.Domain.Events;
using MiniTwitter.Core.Application.Services.Interfaces;
using MiniTwitter.Core.Domain.Entities;

namespace MiniTwitter.Infrastructure.Events.Handlers
{
    public class SendFollowNotificationHandler : IEventHandler<UserFollowedEvent>
    {
        private readonly INotificationService _notificationService;

        public SendFollowNotificationHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(UserFollowedEvent domainEvent)
        {
            var notification = new Notification
            {
                ReceiverUserId = domainEvent.FollowedId,
                SenderUserId = domainEvent.FollowerId,
                Message = $"User {domainEvent.FollowerId} followed you.",
                IsRead = false
            };
            await _notificationService.AddAsync(notification);
        }
    }
}
