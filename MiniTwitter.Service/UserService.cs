using Microsoft.Extensions.Logging;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    public class UserService : GenericService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;

        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository,
                           ILogger<UserService> logger,
                           IPasswordHasher passwordHasher) : base(userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;

        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.FindOneAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.FindOneAsync(u => u.Username == username);
        }

        public async Task<User> IsUserActiveAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User is not active or doesn't exist");
            }

            return user;
        }

        public async Task<User> LoginUserAsync(LoginDto user)
        {
            try
            {
                var foundUser = await _userRepository.FindOneAsync(u => u.Email == user.Email);
                if (foundUser == null || !_passwordHasher.VerifyHashedPassword(user.Password, foundUser.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                return foundUser;
            }
            catch (UnauthorizedAccessException ex)
            {
                //_logger.LogWarning($"Unauthorized access: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred while logging in the user.");
                throw new Exception("An Unexpected error occured. Please try again later. ");
            }
        }

        public async Task<string> RegisterUserAsync(RegisterDto user)
        {
            try
            {
                var userExists = await _userRepository.ExistsAsync(x => x.Email == user.Email || x.Username == user.UserName);
                if (userExists)
                {
                    throw new ApplicationException("Email is already registered.");
                }
                var newUser = new User
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = true,
                    PasswordHash = _passwordHasher.HashPassword(user.Password)
                };
                await _userRepository.AddAsync(newUser);
                return "User Registered Successfully!";
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                throw new Exception("An Unexpected error occured. Please try again later. ");
            }
        }
    }
}
