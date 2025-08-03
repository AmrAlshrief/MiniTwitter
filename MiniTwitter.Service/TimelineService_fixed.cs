using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    public class TimelineService : ITimelineService, IDisposable
    {
        private readonly IGenericService<Tweet> _tweetService;
        private readonly IGenericService<Follow> _followService;
        private readonly IGenericService<User> _userService;
        private readonly ILogger<TimelineService> _logger;
        private bool _disposed = false;

        public TimelineService(
            IGenericService<Tweet> tweetService,
            IGenericService<Follow> followService,
            IGenericService<User> userService,
            ILogger<TimelineService> logger)
        {
            _tweetService = tweetService ?? throw new ArgumentNullException(nameof(tweetService));
            _followService = followService ?? throw new ArgumentNullException(nameof(followService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Initialize CursorUtils with logger
            CursorUtils.Initialize(logger);
        }

        public async Task<TimelineResponse> GetHomeTimelineAsync(int userId, int limit = 20, string? cursor = null)
        {
            _logger.LogInformation("Getting home timeline for user {UserId} with limit {Limit} and cursor {Cursor}", 
                userId, limit, cursor);

            try
            {
                var stopwatch = Stopwatch.StartNew();
                var (cursorDate, cursorId) = CursorUtils.DecodeCursorSafe(cursor);

                _logger.LogDebug("Decoded cursor - Date: {CursorDate}, Id: {CursorId}", cursorDate, cursorId);

                // Get followed user IDs more efficiently
                var following = await _followService.FindAllAsync(f => f.FollowerId == userId);
                var userIds = following.Select(f => f.FollowingId).Append(userId).Distinct().ToList();

                _logger.LogDebug("Found {FollowCount} followed users (including self)", userIds.Count);


                var allTweetsQuery = _tweetService.QueryAll(t => userIds.Contains(t.UserId) && !t.IsDeleted, t => t.User);

                var folteredTweetsQuery = CursorUtils.ApplyCursorFilter(allTweetsQuery, cursorDate, cursorId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ThenByDescending(t => t.Id)
                    .Take(limit + 1);

                var tweetEntities = await folteredTweetsQuery.ToListAsync();

                var tweetList = tweetEntities
                    .Select(t => new TweetDto
                    {
                        Id = t.Id,
                        Content = t.Content,
                        CreatedAt = t.CreatedAt,
                        // Initialize other required properties with default values
                        LikesCount = 0,  // Will be updated by the caller if needed
                        IsLikedByCurrentUser = false,  // Will be updated by the caller if needed
                        User = t.User != null ? new UserPreviewDto
                        {
                            Id = t.User.Id,
                            UserName = t.User.Username,
                            ImageUrl = t.User.ProfilePictureUrl
                        } : new UserPreviewDto()
                    })
                    .ToList();
                
                var hasMore = tweetList.Count() > limit;
                if (hasMore)
                {
                    tweetList.RemoveAt(tweetList.Count() - 1);
                }

                // Create cursor safely
                string? nextCursor = null;
                if (hasMore && tweetList.Any())
                {
                    var lastTweet = tweetList.Last();
                    nextCursor = CreateCursorFromDto(lastTweet);
                }

                var response = new TimelineResponse
                {
                    Tweets = tweetList,
                    NextCursor = nextCursor,
                    HasMore = hasMore
                };

                stopwatch.Stop();
                _logger.LogInformation("Retrieved {Count} tweets in {ElapsedMs}ms. HasMore: {HasMore}", 
                    tweetList.Count, stopwatch.ElapsedMilliseconds, hasMore);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home timeline for user {UserId}", userId);
                throw;
            }
        }

        public async Task<TimelineResponse> GetUserTimelineAsync(string username, int limit = 20, string? cursor = null)
        {
                _logger.LogInformation("Getting user timeline for {Username} with limit {Limit} and cursor {Cursor}", 
                username, limit, cursor);

                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    var user = (await _userService.FindAllAsync(u => u.Username == username)).FirstOrDefault();
                    if (user == null)
                    {
                        _logger.LogWarning("User {Username} not found", username);
                        return new TimelineResponse { Tweets = Enumerable.Empty<TweetDto>(), HasMore = false };
                }

                var (cursorDate, cursorId) = CursorUtils.DecodeCursorSafe(cursor);
                _logger.LogDebug("Decoded cursor - Date: {CursorDate}, Id: {CursorId}", cursorDate, cursorId);

                var allTweetsQuery = _tweetService.QueryAll(t => t.UserId == user.Id && !t.IsDeleted);

                var filteredTweets = CursorUtils.ApplyCursorFilter(allTweetsQuery, cursorDate, cursorId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ThenByDescending(t => t.Id)
                    .Take(limit + 1);

                var tweetEntities = await filteredTweets.ToListAsync();

                var tweetList = tweetEntities
                    .Select(t => new TweetDto
                    {
                        Id = t.Id,
                        Content = t.Content,
                        CreatedAt = t.CreatedAt,
                        LikesCount = 0,
                        IsLikedByCurrentUser = false,
                        User = new UserPreviewDto
                        {
                            Id = user.Id,
                            UserName = user.Username,
                            ImageUrl = user.ProfilePictureUrl
                        }
                    }).ToList();

                var hasMore = tweetList.Count > limit;
                if (hasMore)
                    tweetList.RemoveAt(tweetList.Count - 1);

                string? nextCursor = null;
                if (hasMore && tweetList.Any())
                    nextCursor = CreateCursorFromDto(tweetList.Last());

                stopwatch.Stop();
                _logger.LogInformation("Retrieved {Count} tweets for user {Username} in {ElapsedMs}ms. HasMore: {HasMore}", 
                    tweetList.Count, username, stopwatch.ElapsedMilliseconds, hasMore);

                return new TimelineResponse
                {
                    Tweets = tweetList,
                    NextCursor = nextCursor,
                    HasMore = hasMore
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user timeline for {Username}", username);
                throw;
            }
        }

        private string CreateCursorFromDto(TweetDto dto)
        {
            var raw = $"{dto.CreatedAt:o}|{dto.Id}";
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(raw));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose managed state (managed objects).
            }

            // Free any unmanaged objects here.

            _disposed = true;
        }
    }
}
