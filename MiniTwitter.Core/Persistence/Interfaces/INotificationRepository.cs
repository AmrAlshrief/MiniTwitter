using MiniTwitter.Core.Domain.Entities;
using System.Collections.Generic;
using System;

namespace MiniTwitter.Core.Persistence.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task MarkAsReadAsync(int notificationId); 
}
