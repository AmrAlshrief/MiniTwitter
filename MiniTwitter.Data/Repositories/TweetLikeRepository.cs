using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Data.Repositories
{
    public class TweetLikeRepository : GenericRepository<TweetLike>, ITweetLikeRepository
    {
        public TweetLikeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
