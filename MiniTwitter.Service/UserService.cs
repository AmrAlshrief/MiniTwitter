using Microsoft.Extensions.Logging;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using Microsoft.AspNetCore.Http;
using MiniTwitter.Infrastructure.ExternalServices.CloudinaryService;
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
        private readonly IPasswordHasher _passwordHasher;
    private readonly ICloudinaryService _cloudinaryService;

        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository,
                           ILogger<UserService> logger,
                           IPasswordHasher passwordHasher,
                           ICloudinaryService cloudinaryService) : base(userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await base.FindOneAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await base.FindOneAsync(u => u.Username == username);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            return user;
        }

        public async Task<User> IsUserActiveAsync(int userId)
        {
            var user = await base.GetByIdAsync(userId);
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
                var foundUser = await base.FindOneAsync(u => u.Email == user.Email);
                
                if (foundUser == null || !_passwordHasher.VerifyHashedPassword(user.Password, foundUser.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }
                Console.WriteLine(foundUser);
                return foundUser;
            }
            catch (UnauthorizedAccessException ex)
            {
                //_logger.LogWarning($"Unauthorized access: {ex.Message}");
                //Console.WriteLine("Not valid1");
                throw new Exception(ex.Message);
                
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred while logging in the user.");
                //Console.WriteLine("Not valid2");
                throw new Exception(ex.Message);
                
            }
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Apply changes only for non-null fields
            if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != user.Username)
            {
                // Ensure uniqueness if needed
                bool nameTaken = await _userRepository.ExistsAsync(u => u.Username == dto.UserName && u.Id != userId);
                if (nameTaken)
                    throw new ApplicationException("Username already exists.");

                user.Username = dto.UserName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                bool emailTaken = await _userRepository.ExistsAsync(u => u.Email == dto.Email && u.Id != userId);
                if (emailTaken)
                    throw new ApplicationException("Email already exists.");

                user.Email = dto.Email.Trim();
            }

            await _userRepository.UpdateAsync(user);

            return new UserDto
            {
                Id = user.Id,
                UserName = user.Username,
                Email = user.Email
            };
        }

        public async Task<UserPreviewDto> UpdateProfileImageAsync(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Image file is required.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var imageUrl = await _cloudinaryService.UploadImageAsync(file);
            user.ProfilePictureUrl = imageUrl;
            await _userRepository.UpdateAsync(user);

            return new UserPreviewDto
            {
                Id = user.Id,
                UserName = user.Username,
                ImageUrl = imageUrl
            };
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
                    Role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role,
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
