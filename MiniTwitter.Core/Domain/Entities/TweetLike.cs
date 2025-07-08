using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Domain.Entities
{
    public class TweetLike
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TweetId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Tweet Tweet { get; set; }
        public User User { get; set; }

    }
}
