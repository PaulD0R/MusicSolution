using Microsoft.EntityFrameworkCore;
using MusicService.Application.Interfaces.Repositories;
using MusicService.Domain.Models;
using MusicService.Infrastructure.Contexts;

namespace MusicService.Infrastructure.Repositories;

public class LikeRepository(AppDbContext context) : ILikeRepository
{
    public async Task<bool> AddAsync(Like like)
    {
        try 
        {
            await context.Likes.AddAsync(like);
            return await context.SaveChangesAsync() > 0;
        }
        catch(DbUpdateException) {
            return true;
        }
    }

    public async Task<bool> DeleteAsync(Like like)
    {
        try 
        {
            context.Likes.Remove(like);
            return await context.SaveChangesAsync() > 0;
        }
        catch(DbUpdateException) {
            return true;
        }
    }

    public async Task<IEnumerable<MusicData>> GetLikeMusicByUserIdAsync(string userId)
    {
        return await context.Likes.Where(l => l.UserId == userId).Select(l => l.MusicData!).ToListAsync();
    }
}