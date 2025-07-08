using System;
using MiniTwitter.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniTwitter.Data.Config
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(n => n.IsRead)
                .IsRequired();

            builder.HasOne(n => n.ReceiverUser)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.ReceiverUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.SenderUser)
                .WithMany()
                .HasForeignKey(n => n.SenderUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(n => new { n.ReceiverUserId, n.IsRead })
                .HasDatabaseName("IX_ReceiverUserId_IsRead");
        }
    }

}

