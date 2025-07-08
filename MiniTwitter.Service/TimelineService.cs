using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Persistence.Interfaces;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Domain.Entities;


namespace MiniTwitter.Service
{
    public class TimelineService : ITimelineService
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly IGenericService<Tweet> _genericService;
        private readonly IUserRepository _userRepository;
        public TimelineService(ITweetRepository tweetRepository, IUserRepository userRepository, IGenericService<Tweet> genericService)
        {
            _tweetRepository = tweetRepository;
            _genericService = genericService;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TweetDto>> GetHomeTimelineAsync(int userId, int limit = 20, DateTime? cursor = null)
        {
            // TODO: Implement home timeline logic
            // 1. Get users that the current user follows
            // 2. Get their tweets
            // 3. Apply pagination
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TweetDto>> GetUserTimelineAsync(string username, int limit = 20, DateTime? cursor = null)
        {
            // TODO: Implement user timeline logic
            // 1. Get user by username
            // 2. Get their tweets
            // 3. Apply pagination
            throw new NotImplementedException();
        }
    }
}