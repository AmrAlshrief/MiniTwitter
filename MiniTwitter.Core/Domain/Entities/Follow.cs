using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Domain.Entities
{
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }  // User who follows
        public User Follower { get; set; } = null!;

        public int FollowingId { get; set; }  // User being followed
        public User Following { get; set; } = null!;

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;  // Timestamp
    }
}
