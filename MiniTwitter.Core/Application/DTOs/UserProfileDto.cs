namespace MiniTwitter.Core.Application.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set;}
        public string Bio { get; set; } = null!;
        public string? ProfilePicture { get; set; }
        public List<TweetDto> Tweets { get; set; } = new List<TweetDto>();
    }
}
