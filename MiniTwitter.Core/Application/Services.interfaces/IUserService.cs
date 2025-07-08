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
    public interface IUserService : IGenericService<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> IsUserActiveAsync(int userId);
        Task<string> RegisterUserAsync(RegisterDto user);
        Task<User> LoginUserAsync(LoginDto user);

        // Updates the user's profile image and returns minimal user data for UI refresh
        Task<UserPreviewDto> UpdateProfileImageAsync(int userId, IFormFile file);

        // Updates basic user fields (username, email) and returns updated user details
        Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateDto);

    }
}
