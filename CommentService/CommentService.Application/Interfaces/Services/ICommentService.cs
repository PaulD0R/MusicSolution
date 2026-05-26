using CommentService.Application.Models.Comments;
using CommentService.Domain.Events;
using Microsoft.AspNetCore.Http;

namespace CommentService.Application.Interfaces.Services;

public interface ICommentService
{
    Task AddAsync(IFormFile file, string userId, Guid? musicId = null, Guid? parentId = null);
    Task DeleteAsync(Guid commentId);
    Task DeleteByMusicIdAsync(MusicDeleteEvent deleteEvent);
    Task<IEnumerable<CommentDto>> GetCommentsByMusicIdAsync(Guid musicId);
    Task<IEnumerable<CommentResponseDto>> GetCommentsByParentIdAsync(Guid userId);
    Task<Stream> GetFileByIdAsync(Guid commentId);
    Task CleanAsync();
}