using CommentService.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CommentService.Application.Interfaces.Services;

public interface ICommentFileService
{
    ValueTask<Stream> GetStreamByIdAsync(Guid id);
    Task<Dictionary<Guid, byte[]>> GetPicturesByIdsAsync(IEnumerable<Guid> ids);
    Task AddAsync(IFormFile file, Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteRangeAsync(IEnumerable<Guid> ids);
}