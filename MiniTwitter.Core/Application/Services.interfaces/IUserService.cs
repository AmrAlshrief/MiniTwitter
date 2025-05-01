using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Domain.Entities;
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

    }
}
