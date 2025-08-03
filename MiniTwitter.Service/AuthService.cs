using Microsoft.Extensions.Logging;
using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace MiniTwitter.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        private readonly ICacheService _cacheService;

        public AuthService(IUserRepository userRepository,
                          IPasswordHasher passwordHasher,
                          ILogger<AuthService> logger,
                          ICacheService cacheService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _cacheService = cacheService;
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

        public async Task<string> RegisterUserAsync(RegisterDto user)
        {
            var userExists = await _userRepository.ExistsAsync(x => x.Email == user.Email || x.Username == user.UserName);
            if (userExists)
            {
                throw new ApplicationException("Email or Username is already registered.");
            }
            var newUser = new User
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role,
                IsActive = true,
                PasswordHash = _passwordHasher.HashPassword(user.Password)
            };
            await _userRepository.AddAsync(newUser);
            return "User Registered Successfully!";
        }

        public async Task LogoutAsync(string token)
        {
            _logger.LogInformation("LogoutAsync called");
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            // Extract the jti (JWT ID) from the token
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            _logger.LogInformation($"Extracted JTI from token: {jti}");
            
            if (string.IsNullOrEmpty(jti))
            {
                _logger.LogError("Token does not contain a valid JWT ID");
                throw new ArgumentException("Token does not contain a valid JWT ID.");
            }

            var expiry = jwtToken.ValidTo;
            var ttl = expiry - DateTime.UtcNow;
            var blacklistKey = $"blacklist:{jti}";
            
            _logger.LogInformation($"Adding token to blacklist with key: {blacklistKey}, TTL: {ttl}");

            // Store with the same key format that OnTokenValidated expects
            await _cacheService.SetAsync(blacklistKey, "1", ttl);
            
            _logger.LogInformation($"Token successfully added to blacklist");
        }
    }
}