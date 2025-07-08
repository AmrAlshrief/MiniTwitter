using MiniTwitter.Core.Application.Services.interfaces;
using BCrypt.Net;


namespace MiniTwitter.Infrastructure.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); // Use the fully qualified name directly
        }

        public bool VerifyHashedPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); // Use the fully qualified name directly
        }
    }
}
