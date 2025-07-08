using MiniTwitter.Core.Persistence.Interfaces;
using MiniTwitter.Core.Application.Services.Interfaces;
using System;
using MiniTwitter.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniTwitter.Service;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task AddAsync(Notification notification)
    {
        await _notificationRepository.AddAsync(notification);
    }

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
    {
        return await _notificationRepository.GetByUserIdAsync(userId);
    }

    public async Task<List<Notification>> GetUnreadAsync(int userId)
    {
        var all = await _notificationRepository.GetByUserIdAsync(userId);
        return all.FindAll(n => !n.IsRead);
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId);
    }
}
