using System;
using MiniTwitter.Core.Application.DTOs;

namespace MiniTwitter.Core.Application.Services.interfaces
{
    public interface ITimelineService
    {
        Task<IEnumerable<TweetDto>> GetHomeTimelineAsync(
        int userId, 
        int limit = 20, 
        DateTime? cursor = null);

    Task<IEnumerable<TweetDto>> GetUserTimelineAsync(
        string username, 
        int limit = 20, 
        DateTime? cursor = null);
    }
}


