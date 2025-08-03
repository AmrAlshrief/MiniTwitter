using MiniTwitter.Core.Domain.Entities.auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Bio { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsDeleted { get; set; } = false;
        
        [StringLength(50)]
        public string? Role { get; set; } //Optional
        
        public bool IsActive { get; set; } = true;

        // URL of the user's profile image stored on Cloudinary (optional)
        [Url]
        [StringLength(500)]
        public string? ProfilePictureUrl { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<Tweet> Tweets { get; set; } = new List<Tweet>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<TweetLike> TweetLikes { get; set; } = new List<TweetLike>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
