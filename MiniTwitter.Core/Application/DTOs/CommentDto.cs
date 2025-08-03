namespace MiniTwitter.Core.Application.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserPreviewDto User { get; set; }
    }
}



