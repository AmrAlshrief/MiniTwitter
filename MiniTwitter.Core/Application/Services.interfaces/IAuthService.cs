using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Application.Services.interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterUserAsync(RegisterDto user);
        Task<User> LoginUserAsync(LoginDto user);
        Task LogoutAsync(string token);
    }
}