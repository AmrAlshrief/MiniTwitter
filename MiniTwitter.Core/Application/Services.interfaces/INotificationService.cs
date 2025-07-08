using System;
using MiniTwitter.Core.Domain.Entities; 

namespace MiniTwitter.Core.Application.Services.Interfaces;

public interface INotificationService
{
    Task AddAsync(Notification notification);
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
    Task<List<Notification>> GetUnreadAsync(int userId);
}
