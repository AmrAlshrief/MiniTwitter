using MiniTwitter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Application.Services.interfaces
{
    public interface ITweetLikeService
    {
        Task<bool> LikeTweetAsync(int userId, int tweetId);
        Task<bool> UnlikeTweetAsync(int userId, int tweetId);
        Task<bool> IsTweetLikedAsync(int userId, int tweetId);
        Task<IEnumerable<TweetLike>> GetLikesByTweetIdAsync(int tweetId);
        Task<IEnumerable<TweetLike>> GetLikesByUserIdAsync(int userId);
        Task<int> GetLikesCountAsync(int tweetId);
    }
    
}
