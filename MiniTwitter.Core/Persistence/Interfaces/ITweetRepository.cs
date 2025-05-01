using MiniTwitter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Persistence.Interfaces
{
    public interface ITweetRepository : IGenericRepository<Tweet>
    {
        //Task<IEnumerable<Tweet>> GetTweetsByUserIdAsync(int userId);
        //Task<IEnumerable<Tweet>> GetTweetsByUserIdsAsync(IEnumerable<int> userIds);
        //Task<IEnumerable<Tweet>> GetTweetsByUserFollowingAsync(int userId);
        //Task<IEnumerable<Tweet>> GetTweetsByUserFollowingAndUserAsync(int userId, int targetUserId);
        //Task<IEnumerable<Tweet>> GetTweetsByUserAndHashtagAsync(int userId, string hashtag);
        //Task<IEnumerable<Tweet>> GetTweetsByHashtagAsync(string hashtag);
        //Task<IEnumerable<Tweet>> GetTweetsByMentionAsync(int userId);
        //Task<IEnumerable<Tweet>> GetTweetsByMentionAndUserAsync(int userId, int targetUserId);
        //Task<IEnumerable<Tweet>> GetTweetsByMentionAndHashtagAsync(int userId, string hashtag);
        //Task<IEnumerable<Tweet>> GetTweetsByMentionAndUserAndHashtagAsync(int userId, int targetUserId, string hashtag);
    }
}
