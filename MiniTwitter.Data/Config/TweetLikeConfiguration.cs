using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniTwitter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Data.Config
{
    public class TweetLikeConfiguration : IEntityTypeConfiguration<TweetLike>
    {
        public void Configure(EntityTypeBuilder<TweetLike> builder)
        {


            builder.HasIndex(l => new { l.UserId, l.TweetId }).IsUnique();

            builder.HasOne(l => l.User)
                .WithMany(u => u.TweetLikes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(l => l.Tweet)
                .WithMany(t => t.TweetLikes)
                .HasForeignKey(l => l.TweetId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
