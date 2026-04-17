using CommentService.Domain.Models;

namespace CommentService.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetCommentsByMusicIdAsync(Guid musicId);
    Task<IEnumerable<Comment>> GetCommentsByParentIdAsync(Guid parentId);
    Task<IEnumerable<Guid>> GetAllCommentIdsAsync();
    Task<Comment?> GetByIdAsync(Guid id);
    Task<Comment?> AddAsync(Comment comment);
    Task<bool> DeleteAsync(Comment comment);
    Task<bool> DeleteByMusicIdAsync(Guid musicId);
    Task<bool> IsExistAsync(Guid id);
}