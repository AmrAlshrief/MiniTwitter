using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Application.Events;
using MiniTwitter.Core.Domain.Events;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    public class TweetService : ITweetService
    {
            private readonly IGenericService<Tweet> _genericService;
            private readonly ITweetRepository _tweetRepository;
            private readonly ITweetLikeRepository _tweetLikeRepository;
            private readonly IEventDispatcher _eventDispatcher;
        public TweetService(ITweetRepository tweetRepository, ITweetLikeRepository tweetLikeRepository, IEventDispatcher eventDispatcher, IGenericService<Tweet> genericService)

        {
            _genericService = genericService;

            _tweetRepository = tweetRepository;
            _tweetLikeRepository = tweetLikeRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<Tweet> AddTweetAsync(string content, int userId) 
        {
            var tweet = new Tweet
            {
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _genericService.AddAsync(tweet);
            return tweet;
        }
        

        public async Task<IEnumerable<Tweet>> GetTweetsByUserIdAsync(int userId)
        {
            return await _genericService.FindAllAsync(t => t.UserId == userId && !t.IsDeleted);
        }

        public async Task<Tweet> UpdateTweetAsync(int tweetId, string newContent, int userId) 
        {
            var tweet = await _genericService.GetByIdAsync(tweetId);
            if (tweet == null || tweet.IsDeleted)
                throw new Exception("tweet not found");

            if (tweet.UserId != userId)
                throw new UnauthorizedAccessException("unable to edit");

            tweet.Content = newContent;

            await _genericService.UpdateAsync(tweet);

            return tweet;
        }

        public async Task DeleteTweetAsync(int tweetId, int userId) 
        {
            var tweet = await _genericService.GetByIdAsync(tweetId);

            if (tweet == null || tweet.IsDeleted)
                throw new Exception("Tweet not found");
            if(tweet.UserId != userId)
                throw new UnauthorizedAccessException("unable to delete");

            tweet.IsDeleted = true;

            await _genericService.UpdateAsync(tweet);
        }
        
        public async Task LikeTweetAsync(int tweetId, int userId)
        {
            var tweet = await _genericService.GetByIdAsync(tweetId);
            if (tweet == null || tweet.IsDeleted)
                throw new Exception("Tweet not found");

            // Check if already liked
            var alreadyLiked = await _tweetLikeRepository.ExistsAsync(l => l.TweetId == tweetId && l.UserId == userId);
            if (alreadyLiked)
                throw new InvalidOperationException("You have already liked this tweet.");

            var like = new TweetLike
            {
                TweetId = tweetId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _tweetLikeRepository.AddAsync(like);
            await _eventDispatcher.DispatchAsync(new TweetLikedEvent(userId, tweetId));
        }
    }
}
