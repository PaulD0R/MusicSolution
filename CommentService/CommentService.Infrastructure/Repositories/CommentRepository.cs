using CommentService.Application.Interfaces.Repositories;
using CommentService.Domain.Events;
using CommentService.Domain.Models;
using CommentService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Infrastructure.Repositories;

public class CommentRepository(AppDbContext context) : ICommentRepository
{
    public async Task<IEnumerable<Comment>> GetCommentsByMusicIdAsync(Guid musicId)
    {
        var data = await context.Comments
            .Where(c => c.MusicId == musicId)
            .Include(c => c.Person)
            .Select(c => new 
            {
                Comment = c,
                c.Comments.Count
            })
            .ToListAsync();

        return data.Select(x => 
        {
            x.Comment.ResponseCount = x.Count;
            return x.Comment;
        });
    }

    public async Task<IEnumerable<Comment>> GetCommentsByParentIdAsync(Guid parentId)
    {
        return await context.Comments.Where(c => c.ParentId == parentId)
            .Include(c => c.Person).ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetAllCommentIdsAsync()
    {
        return await context.Comments.Select(c => c.Id).ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await context.Comments.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Comment?> AddAsync(Comment comment)
    {
        await context.Comments.AddAsync(comment);
        return await context.SaveChangesAsync() > 0 ? comment : null;
    }

    public async Task<bool> DeleteAsync(Comment comment)
    {
        context.Comments.Remove(comment);
        comment.DeleteEvents.Add(new CommentDeleteEvent{CommentId = comment.Id});
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteByMusicIdAsync(Guid musicId)
    {
        var comments = await context.Comments
            .Where(c => c.MusicId == musicId)
            .ToListAsync();

        if (comments.Count == 0) return true;

        context.Comments.RemoveRange(comments);

        foreach (var comment in comments)
            comment.DeleteEvents.Add(new CommentDeleteEvent{CommentId = comment.Id});
        
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> IsExistAsync(Guid id)
    {
        return await context.Comments.AnyAsync(c => c.Id == id);
    }
}