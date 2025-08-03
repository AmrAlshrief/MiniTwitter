using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.Core.Application.DTOs
{
    // Fields allowed to be updated on a user's profile (except profile image which has a dedicated endpoint)
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Bio { get; set; }
    }
}
