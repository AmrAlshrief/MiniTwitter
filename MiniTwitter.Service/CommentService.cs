using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Application.Services.interfaces;
using MiniTwitter.Core.Domain.Entities;
using MiniTwitter.Core.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Service
{
    
    public class CommentService : ICommentService
    {
        private readonly IGenericService<Comment> _genericService;
        private readonly IGenericService<User> _userService;
        private readonly IGenericService<Tweet> _tweetService;
        private readonly ICommentRepository _commentRepository;

        public CommentService(
            IGenericService<Comment> genericService,
            IGenericService<User> userService,
            IGenericService<Tweet> tweetService,
            ICommentRepository commentRepository)
        {
            _genericService = genericService;
            _userService = userService;
            _tweetService = tweetService;
            _commentRepository = commentRepository;
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto)
        {
            // Validate that the tweet exists
            var tweet = await _tweetService.GetByIdAsync(createCommentDto.TweetId);
            if (tweet == null || tweet.IsDeleted)
                throw new Exception("Tweet not found");

            // Validate that the user exists
            var user = await _userService.GetByIdAsync(createCommentDto.UserId);
            if (user == null)
                throw new Exception("User not found");

            var comment = new Comment
            {
                Content = createCommentDto.Content,
                UserId = createCommentDto.UserId,
                TweetId = createCommentDto.TweetId,
                CreatedAt = DateTime.UtcNow
            };

            await _genericService.AddAsync(comment);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                User = new UserPreviewDto
                {
                    Id = user.Id,
                    UserName = user.Username,
                    ImageUrl = user.ProfilePictureUrl
                }
            };
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByTweetIdAsync(int tweetId)
        {
            var comments = await _genericService.FindAllAsync(
                c => c.TweetId == tweetId && !c.IsDeleted,
                c => c.User
            );

            return comments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                User = new UserPreviewDto
                {
                    Id = comment.User.Id,
                    UserName = comment.User.Username,
                    ImageUrl = comment.User.ProfilePictureUrl
                }
            }).OrderBy(c => c.CreatedAt);
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _genericService.GetByIdAsync(commentId);
            if (comment == null || comment.IsDeleted)
                throw new Exception("Comment not found");

            comment.IsDeleted = true;
            await _genericService.UpdateAsync(comment);
        }

        public async Task<CommentDto> UpdateCommentAsync(int commentId, UpdateCommentDto updateCommentDto)
        {
            var comment = await _genericService.FindOneAsync(
                c => c.Id == commentId && !c.IsDeleted,
                c => c.User
            );

            if (comment == null)
                throw new Exception("Comment not found");

            comment.Content = updateCommentDto.Content;
            await _genericService.UpdateAsync(comment);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                User = new UserPreviewDto
                {
                    Id = comment.User.Id,
                    UserName = comment.User.Username,
                    ImageUrl = comment.User.ProfilePictureUrl
                }
            };
        }
    }
    
}
