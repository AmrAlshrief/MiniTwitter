using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Data.Repositories
{
    public class CommentLikeRepository : GenericRepository<CommentLike>, ICommentLikeRepository
    {
        public CommentLikeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
