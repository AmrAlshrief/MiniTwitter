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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenericService<User> _genericService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<UserService> _logger;
        private readonly ITweetLikeService _tweetLikeService;

        public UserService(IUserRepository userRepository,
                           IGenericService<User> genericService,
                           ILogger<UserService> logger,
                           ICloudinaryService cloudinaryService,
                           ITweetLikeService tweetLikeService)
        {
            _userRepository = userRepository;
            _genericService = genericService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
            _tweetLikeService = tweetLikeService;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _genericService.FindOneAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _genericService.FindOneAsync(u => u.Username == username);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            return user;
        }

        public async Task<User> IsUserActiveAsync(int userId)
        {
            var user = await _genericService.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User is not active or doesn't exist");
            }

            return user;
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            
            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Bio))
                user.Bio = dto.Bio.Trim();

            // Apply changes only for non-null fields
            // if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != user.Username)
            // {
            //     bool nameTaken = await _userRepository.ExistsAsync(u => u.Username == dto.UserName && u.Id != userId);
            //     if (nameTaken)
            //         throw new ApplicationException("Username already exists.");

            //     user.Username = dto.UserName.Trim();
            // }

            // if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            // {
            //     bool emailTaken = await _userRepository.ExistsAsync(u => u.Email == dto.Email && u.Id != userId);
            //     if (emailTaken)
            //         throw new ApplicationException("Email already exists.");

            //     user.Email = dto.Email.Trim();
            // }

            await _userRepository.UpdateAsync(user);

            return new UserDto
            {
                Id = user.Id,
                UserName = user.Username,
                Email = user.Email,
                Name = $"{user.FirstName} {user.LastName}".Trim(),
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
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



        public async Task<UserProfileDto?> GetUserProfileAsync(string username)
        {
            var user = await _userRepository.FindOneAsync(
                u => u.Username == username,
                u => u.Tweets
            );

            if (user == null) return null;

        var tweets = new List<TweetDto>();
        foreach (var tweet in user.Tweets.Where(t => !t.IsDeleted))
        {
            var likesCount = await _tweetLikeService.GetLikesCountAsync(tweet.Id);
            tweets.Add(new TweetDto
            {
                Id = tweet.Id,
                Content = tweet.Content,
                CreatedAt = tweet.CreatedAt,
                LikesCount = likesCount,
            });
        }

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Name = $"{user.FirstName} {user.LastName}".Trim(),
            Bio = user.Bio,
            ProfilePicture = user.ProfilePictureUrl,
            Tweets = tweets
        };
}

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _genericService.GetAllAsync();
        }
    }
}
