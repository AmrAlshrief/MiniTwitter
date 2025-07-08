using System.Threading.Tasks;
using MiniTwitter.Core.Application.Events;
using MiniTwitter.Core.Domain.Events;
using MiniTwitter.Core.Persistence.Interfaces;
using MiniTwitter.Core.Application.Services.Interfaces;
using MiniTwitter.Core.Domain.Entities;

namespace MiniTwitter.Infrastructure.Events.Handlers
{
    public class SendRetweetNotificationHandler : IEventHandler<TweetRetweetedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepo;
        private readonly ITweetRepository _tweetRepo;

        public SendRetweetNotificationHandler(
            INotificationService notificationService,
            IUserRepository userRepo,
            ITweetRepository tweetRepo)
        {
            _notificationService = notificationService;
            _userRepo = userRepo;
            _tweetRepo = tweetRepo;
        }

        public async Task HandleAsync(TweetRetweetedEvent domainEvent)
        {
            var retweeter = await _userRepo.GetByIdAsync(domainEvent.RetweeterId);
            var tweet = await _tweetRepo.GetByIdAsync(domainEvent.TweetId);

            var notification = new Notification
            {
                ReceiverUserId = tweet.UserId,
                SenderUserId = retweeter.Id,
                Message = $"{retweeter.Username} retweeted your tweet.",
                IsRead = false
            };
            await _notificationService.AddAsync(notification);
        }
    }
}
