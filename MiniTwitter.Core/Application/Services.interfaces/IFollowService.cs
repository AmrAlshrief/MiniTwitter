using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Application.Services.interfaces
{
    public interface IFollowService
    {
        Task FollowUserAsync(int followerId, int followingId);
        Task UnfollowUserAsync(int followerId, int followingId);
        Task<bool> IsFollowingAsync(int followerId, int followingId);
        Task<IEnumerable<UserDto>> GetFollowersAsync(int userId);
        Task<IEnumerable<UserDto>> GetFollowingAsync(int userId);


    }
}
