using System;
using MiniTwitter.Core.Application.DTOs;

namespace MiniTwitter.Core.Application.Services.interfaces
{
    public interface ITimelineService
    {
        Task<TimelineResponse> GetHomeTimelineAsync(
            int userId,
            int limit = 20,
            string? cursor = null);

        Task<TimelineResponse> GetUserTimelineAsync(
            string username,
            int limit = 20,
            string? cursor = null);
    }
}


