using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.Core.Application.DTOs
{
    // Fields allowed to be updated on a user's profile (except profile image which has a dedicated endpoint)
    public class UpdateUserDto
    {
        [MaxLength(50)]
        public string? UserName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
