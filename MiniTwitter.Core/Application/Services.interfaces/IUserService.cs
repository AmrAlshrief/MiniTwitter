using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Application.Services.interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> IsUserActiveAsync(int userId);
        Task<UserPreviewDto> UpdateProfileImageAsync(int userId, IFormFile file);
        Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateDto);
        Task<UserProfileDto?> GetUserProfileAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();

    }
}
