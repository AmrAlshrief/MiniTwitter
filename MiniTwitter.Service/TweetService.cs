using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    public class TweetService :GenericService<Tweet>, ITweetService
    {
        private readonly IGenericRepository<Tweet> _tweetRepository;
        public TweetService(IGenericRepository<Tweet> tweetRepository) : base(tweetRepository)
        {
            _tweetRepository = tweetRepository;
        }

        public async Task<IEnumerable<Tweet>> GetTweetsByUserIdAsync(int userId)
        {
            return await _tweetRepository.GetAllAsync();
        }
    }
}
