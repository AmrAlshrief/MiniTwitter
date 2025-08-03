// using Microsoft.Extensions.Logging;
// using MiniTwitter.Core.Application.DTOs;
// using MiniTwitter.Core.Application.Services.interfaces;
// using MiniTwitter.Core.Domain.Entities;
// using MiniTwitter.Core.Persistence.Interfaces;
// using System;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;

// namespace MiniTwitter.Service
// {
//     public class TimelineService : ITimelineService, IDisposable
//     {
//         private readonly IGenericService<Tweet> _tweetService;
//         private readonly IGenericService<Follow> _followService;
//         private readonly IGenericService<User> _userService;
//         private readonly ILogger<TimelineService> _logger;
//         private bool _disposed = false;

//         public TimelineService(
//             IGenericService<Tweet> tweetService,
//             IGenericService<Follow> followService,
//             IGenericService<User> userService,
//             ILogger<TimelineService> logger)
//         {
//             _tweetService = tweetService ?? throw new ArgumentNullException(nameof(tweetService));
//             _followService = followService ?? throw new ArgumentNullException(nameof(followService));
//             _userService = userService ?? throw new ArgumentNullException(nameof(userService));
//             _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
//             // Initialize CursorUtils with logger
//             CursorUtils.Initialize(logger);
//         }

//         public async Task<TimelineResponse> GetHomeTimelineAsync(int userId, int limit = 20, string? cursor = null)
//         {
//             _logger.LogInformation("Getting home timeline for user {UserId} with limit {Limit} and cursor {Cursor}", 
//                 userId, limit, cursor);

//             try
//             {
//                 var stopwatch = Stopwatch.StartNew();
//                 var (cursorDate, cursorId) = CursorUtils.DecodeCursorSafe(cursor);

//                 _logger.LogDebug("Decoded cursor - Date: {CursorDate}, Id: {CursorId}", cursorDate, cursorId);

//                 // Get followed user IDs more efficiently
//                 var following = await _followService.FindAllAsync(f => f.FollowerId == userId);
//                 var userIds = following.Select(f => f.FollowingId).Append(userId).Distinct().ToList();

//                 _logger.LogDebug("Found {FollowCount} followed users (including self)", userIds.Count);

//                 // Get all matching tweets first (this is less efficient but works with the current IGenericService)
//                 var allTweets = (await _tweetService.FindAllAsync(t => userIds.Contains(t.UserId) && !t.IsDeleted))
//                     .AsQueryable();

//                 // Apply cursor filter
//                 var filteredTweets = CursorUtils.ApplyCursorFilter(allTweets, cursorDate, cursorId)
//                     .OrderByDescending(t => t.CreatedAt)
//                     .ThenByDescending(t => t.Id);

//                 // Get one more than requested to determine if there are more items
//                 var tweetList = filteredTweets
//                     .Take(limit + 1)
//                     .Select(t => new TweetDto
//                     {
//                         Id = t.Id,
//                         Content = t.Content,
//                         CreatedAt = t.CreatedAt,
//                         // Initialize other required properties with default values
//                         LikeCount = 0,  // Will be updated by the caller if needed
//                         IsLikedByCurrentUser = false,  // Will be updated by the caller if needed
//                         User = new UserPreviewDto()  // Will be updated by the caller if needed
//                     })
//                     .ToList();

//                 var hasMore = tweetList.Count > limit;
//                 if (hasMore)
//                 {
//                     tweetList.RemoveAt(tweetList.Count - 1);
//                 }

//                 var response = new TimelineResponse
//                 {
//                     Tweets = tweetList,
//                     NextCursor = hasMore && tweetList.Any() ? CursorUtils.CreateCursor(new Tweet 
//                 { 
//                     Id = tweetList.Last().Id, 
//                     CreatedAt = tweetList.Last().CreatedAt 
//                 }) : null,
//                     HasMore = hasMore
//                 };

//                 stopwatch.Stop();
//                 _logger.LogInformation("Retrieved {Count} tweets in {ElapsedMs}ms. HasMore: {HasMore}", 
//                     tweetList.Count, stopwatch.ElapsedMilliseconds, hasMore);

//                 return response;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error getting home timeline for user {UserId}", userId);
//                 throw;
//             }
//         }

//         public async Task<TimelineResponse> GetUserTimelineAsync(string username, int limit = 20, string? cursor = null)
//         {
//             _logger.LogInformation("Getting user timeline for {Username} with limit {Limit} and cursor {Cursor}", 
//                 username, limit, cursor);

//             try
//             {
//                 var stopwatch = Stopwatch.StartNew();
//                 var user = (await _userService.FindAllAsync(u => u.Username == username)).FirstOrDefault();
//                 if (user == null)
//                 {
//                     _logger.LogWarning("User {Username} not found", username);
//                     return new TimelineResponse { Tweets = Enumerable.Empty<TweetDto>(), HasMore = false };
//                 }

//                 var (cursorDate, cursorId) = CursorUtils.DecodeCursorSafe(cursor);
//                 _logger.LogDebug("Decoded cursor - Date: {CursorDate}, Id: {CursorId}", cursorDate, cursorId);

//                 // Get all matching tweets first (this is less efficient but works with the current IGenericService)
//                 var allTweets = (await _tweetService.FindAllAsync(t => t.UserId == user.Id && !t.IsDeleted))
//                     .AsQueryable();

//                 // Apply cursor filter
//                 var filteredTweets = CursorUtils.ApplyCursorFilter(allTweets, cursorDate, cursorId)
//                     .OrderByDescending(t => t.CreatedAt)
//                     .ThenByDescending(t => t.Id);

//                 // Get one more than requested to determine if there are more items
//                 var tweetList = filteredTweets
//                     .Take(limit + 1)
//                     .Select(t => new TweetDto
//                     {
//                         Id = t.Id,
//                         Content = t.Content,
//                         CreatedAt = t.CreatedAt,
//                         // Initialize other required properties with default values
//                         LikeCount = 0,  // Will be updated by the caller if needed
//                         IsLikedByCurrentUser = false,  // Will be updated by the caller if needed
//                         User = new UserPreviewDto()  // Will be updated by the caller if needed
//                     })
//                     .ToList();

//                 var hasMore = tweetList.Count > limit;
//                 if (hasMore)
//                 {
//                     tweetList.RemoveAt(tweetList.Count - 1);
//                 }

//                 var response = new TimelineResponse
//                 {
//                     Tweets = tweetList,
//                     NextCursor = hasMore && tweetList.Any() ? CursorUtils.CreateCursor(new Tweet 
//                 { 
//                     Id = tweetList.Last().Id, 
//                     CreatedAt = tweetList.Last().CreatedAt 
//                 }) : null,
//                     HasMore = hasMore
//                 };

//                 stopwatch.Stop();
//                 _logger.LogInformation("Retrieved {Count} tweets for user {Username} in {ElapsedMs}ms. HasMore: {HasMore}", 
//                     tweetList.Count, username, stopwatch.ElapsedMilliseconds, hasMore);

//                 return response;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error getting user timeline for {Username}", username);
//                 throw;
//             }
//         }

//         public void Dispose()
//         {
//             Dispose(true);
//             GC.SuppressFinalize(this);
//         }

//         protected virtual void Dispose(bool disposing)
//         {
//             if (_disposed)
//                 return;

//             if (disposing)
//             {
//                 // Dispose managed state (managed objects).
//             }

//             // Free any unmanaged objects here.

//             _disposed = true;
//         }
//     }
// }