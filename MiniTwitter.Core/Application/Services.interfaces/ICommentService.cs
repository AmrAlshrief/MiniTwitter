using MiniTwitter.Core.Application.DTOs;
using MiniTwitter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Application.Services.interfaces
{
    
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto);
        Task<IEnumerable<CommentDto>> GetCommentsByTweetIdAsync(int tweetId);
        Task DeleteCommentAsync(int commentId);
        Task<CommentDto> UpdateCommentAsync(int commentId, UpdateCommentDto updateCommentDto);
    }
}
