using System;

namespace MiniTwitter.Core.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; }
    public int? ReceiverUserId { get; set; }
    public int? SenderUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;   


    public User ReceiverUser { get; set; }
    public User? SenderUser { get; set; }
    
}
