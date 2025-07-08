using System;

namespace MiniTwitter.Core.Application.DTOs
{
    public class UserPreviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
