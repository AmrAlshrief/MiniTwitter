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
    public class FollowConfiguration : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.HasKey(f => f.Id);
            builder.HasOne(f => f.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(f => f.Following)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
            

            builder.HasIndex(f => f.FollowerId)
                .HasDatabaseName("IX_FollowerId");
                
            builder.HasIndex(f => f.FollowingId)
                .HasDatabaseName("IX_FollowingId");

            builder.HasIndex(f => new { f.FollowerId, f.FollowingId }).IsUnique()
                .HasDatabaseName("IX_FollowerId_FollowingId");
        }
    }
}
