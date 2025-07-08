using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Persistence.Interfaces;
using MiniTwitter.Core.Application.Events;
using MiniTwitter.Core.Domain.Events;
using System.Linq.Expressions;

namespace MiniTwitter.Service
{
    public class TweetLikeService : ITweetLikeService
    {
        private readonly ITweetLikeRepository _tweetLikeRepository;
        private readonly IGenericService<TweetLike> _genericService;
        private readonly IEventDispatcher _eventDispatcher;
        public TweetLikeService(ITweetLikeRepository tweetLikeRepository, IGenericService<TweetLike> genericService, IEventDispatcher eventDispatcher)
        {
            _tweetLikeRepository = tweetLikeRepository;
            _genericService = genericService;
            _eventDispatcher = eventDispatcher;

        }
        public async Task<IEnumerable<TweetLike>> GetLikesByTweetIdAsync(int tweetId)
        {
            return await _genericService.FindAllAsync(l => l.TweetId == tweetId);
        }

        public async Task<IEnumerable<TweetLike>> GetLikesByUserIdAsync(int userId)
        {
            return await _genericService.FindAllAsync(l => l.UserId == userId);
        }

        public async Task<bool> IsTweetLikedAsync(int userId, int tweetId)
        {
            return await _genericService.ExistsAsync(
                like => like.UserId == userId &&
                        like.TweetId == tweetId);
        }

        public async Task<bool> LikeTweetAsync(int userId, int tweetId)
        {
            if (!await IsTweetLikedAsync(userId, tweetId))
            {
                var tweetLike = new TweetLike
                {
                    UserId = userId,
                    TweetId = tweetId,
                    CreatedAt = DateTime.UtcNow
                };
                await _genericService.AddAsync(tweetLike);
                await _eventDispatcher.DispatchAsync(new TweetLikedEvent(userId, tweetId));

                return true;
            }
            return false;
        }

        public async Task<bool> UnlikeTweetAsync(int userId, int tweetId)
        {
            var like = await _genericService.FindOneAsync(l => l.TweetId == tweetId && l.UserId == userId);
            if (like != null)
            {
                await _genericService.DeleteAsync(like);
                return true;
            }

            return false;
        }

        public async Task<int> GetLikesCountAsync(int tweetId)
        {
            var likes = await _genericService.FindAllAsync(l => l.TweetId == tweetId);
            return likes.Count();
        }
    }
    
    
}