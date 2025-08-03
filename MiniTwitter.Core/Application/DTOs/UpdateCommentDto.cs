using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.Core.Application.DTOs
{
    public class UpdateCommentDto
    {
        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;
    }
}