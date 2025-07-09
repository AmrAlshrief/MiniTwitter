using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Persistence.Interfaces;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;


namespace MiniTwitter.Service
{
    public class TimelineService : ITimelineService
    {
        private readonly IGenericService<Tweet> _tweetService;
        private readonly IGenericService<Follow> _followService;
        private readonly IGenericService<User> _userService;

        public TimelineService(
            IGenericService<Tweet> tweetService,
            IGenericService<Follow> followService,
            IGenericService<User> userService)
        {
            _tweetService = tweetService;
            _followService = followService;
            _userService = userService;
        }

        public async Task<TimelineResponse> GetHomeTimelineAsync(int userId, int limit = 20, string? cursor = null)
        {
            var (cursorDate, cursorId) = CursorUtils.DecodeCursorSafe(cursor);

            var following = await _followService.FindAllAsync(f => f.FollowerId == userId);
            var userIds = following.Select(f => f.FollowingId).ToList();
            userIds.Add(userId); // include self

            var tweets = (await _tweetService.FindAllAsync(t => userIds.Contains(t.UserId) && !t.IsDeleted))
                .AsQueryable();

            tweets = CursorUtils.ApplyCursorFilter(tweets, cursorDate, cursorId)
                .OrderByDescending(t => t.CreatedAt)
                .ThenByDescending(t => t.Id);

            var tweetList = tweets.Take(limit + 1).ToList();
            var hasMore = tweetList.Count > limit;
            if (hasMore)
                tweetList.RemoveAt(tweetList.Count - 1);

            return new TimelineResponse
            {
                Tweets = tweetList.Select(t => new TweetDto
                {
                    Id = t.Id,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
                    // Map user info if needed
                }),
                NextCursor = CursorUtils.CreateCursor(tweetList.LastOrDefault()),
                HasMore = hasMore
            };
        }

        public async Task<TimelineResponse> GetUserTimelineAsync(string username, int limit = 20, string? cursor = null)
        {
            var user = (await _userService.FindAllAsync(u => u.Username == username)).FirstOrDefault();
            if (user == null)
                return new TimelineResponse { Tweets = Enumerable.Empty<TweetDto>(), HasMore = false };

            var (cursorDate, cursorId) = CursorUtils.DecodeCursorSafe(cursor);

            var tweets = (await _tweetService.FindAllAsync(t => t.UserId == user.Id && !t.IsDeleted))
                .AsQueryable();

            tweets = CursorUtils.ApplyCursorFilter(tweets, cursorDate, cursorId)
                .OrderByDescending(t => t.CreatedAt)
                .ThenByDescending(t => t.Id);

            var tweetList = tweets.Take(limit + 1).ToList();
            var hasMore = tweetList.Count > limit;
            if (hasMore)
                tweetList.RemoveAt(tweetList.Count - 1);

            return new TimelineResponse
            {
                Tweets = tweetList.Select(t => new TweetDto
                {
                    Id = t.Id,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
                    // Map user info if needed
                }),
                NextCursor = CursorUtils.CreateCursor(tweetList.LastOrDefault()),
                HasMore = hasMore
            };
        }
    }
}