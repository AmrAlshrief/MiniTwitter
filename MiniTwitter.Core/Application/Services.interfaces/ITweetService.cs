using MiniTwitter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Application.Services.interfaces
{
    public interface ITweetService
    {
        Task<IEnumerable<Tweet>> GetTweetsByUserIdAsync(int userId);
        Task<Tweet> AddTweetAsync(string content, int userId);
        Task<Tweet> UpdateTweetAsync(int tweetId, string newContent, int userId);
        Task DeleteTweetAsync(int tweetId, int userId);
    }
}
