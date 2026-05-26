using CommentService.Application.Interfaces.Caches;
using CommentService.Application.Interfaces.Repositories;
using CommentService.Application.Interfaces.Services;
using CommentService.Application.Mappers;
using CommentService.Application.Models.Comments;
using CommentService.Domain.Events;
using CommentService.Domain.Exceptions;
using CommentService.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommentService.Application.Services;

public class CommentService(
    ICommentRepository commentRepository,
    ICommentFileService commentFileService,
    ICachingService cachingService,
    ILogger<CommentService> logger)
    : ICommentService
{
    public async Task AddAsync(IFormFile file, string userId, Guid? musicId = null, Guid? parentId = null)
    {
        var comment = await commentRepository.AddAsync(new Comment
        {
            MusicId = musicId,
            PersonId = userId,
            ParentId = parentId,
            CreatedAt = DateTime.UtcNow
        });
        if (comment == null)
            throw new InternalServerErrorException("Comment could not be created");
        
        try
        {
            await commentFileService.AddAsync(file, comment.Id);   
        }
        catch
        {
            await commentRepository.DeleteAsync(comment);
            throw;
        } 
    }

    public async Task DeleteAsync(Guid commentId)
    {
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment == null)
            throw new NotFoundException("Comment could not be found");
        if (!await commentRepository.DeleteAsync(comment))
            throw new InternalServerErrorException("Comment could not be deleted");
    }

    public async Task DeleteByMusicIdAsync(MusicDeleteEvent deleteEvent)
    {
        if (!await commentRepository.DeleteByMusicIdAsync(deleteEvent.MusicId))
            throw new InternalServerErrorException("Comment could not be deleted");
        logger.LogInformation("Comment with music id {MusicId} deleted", deleteEvent.MusicId);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByMusicIdAsync(Guid musicId)
    {
        var comments = (await commentRepository.GetCommentsByMusicIdAsync(musicId)).ToList();
        var pictures = await commentFileService.GetPicturesByIdsAsync(comments.Select(c => c.Id));
        return comments.Select(c => c.ToCommentDto(pictures[c.Id]));
    }

    public async Task<IEnumerable<CommentResponseDto>> GetCommentsByParentIdAsync(Guid userId)
    {
        var comments = (await commentRepository.GetCommentsByParentIdAsync(userId)).ToList();
        var pictures = await commentFileService.GetPicturesByIdsAsync(comments.Select(c => c.Id));
        return comments.Select(c => c.ToCommentResponseDto(pictures[c.Id]));
    }

    public async Task<Stream> GetFileByIdAsync(Guid commentId)
    {
        if (!await commentRepository.IsExistAsync(commentId))
            throw new NotFoundException("Comment could not be found");
        return await commentFileService.GetStreamByIdAsync(commentId);
    }

    public async Task CleanAsync()
    {
        var ids = await commentRepository.GetAllCommentIdsAsync();
        await commentFileService.DeleteRangeAsync(ids);
    }
}