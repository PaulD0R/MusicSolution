using Microsoft.EntityFrameworkCore;
using MusicService.Application.DTOs.Helpers;
using MusicService.Application.Interfaces.Repositories;
using MusicService.Domain.Models;
using MusicService.Infrastructure.Contexts;

namespace MusicService.Infrastructure.Repositories;

public class MusicDataRepository(AppDbContext context) : IMusicDataRepository
{
    public async Task<IEnumerable<MusicData>> GetAllAsync(MusicFindRequest findRequest)
    {
        if (findRequest.Name == null) return await context.MusicData.AsNoTracking().ToListAsync();
        return await context.MusicData
            .Where(m => EF.Functions.TrigramsSimilarity(m.Name, findRequest.Name) > 0.3)
            .OrderByDescending(m => EF.Functions.TrigramsSimilarity(m.Name, findRequest.Name))
            .AsNoTracking().ToListAsync();
    }

    public Task<MusicData?> GetByIdAsync(Guid id)
    {
        return context.MusicData.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MusicData?> AddAsync(MusicData musicData)
    {
        context.MusicData.Add(musicData);
        return await context.SaveChangesAsync() > 0 ? musicData : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await context.MusicData.Where(m => m.Id == id).ExecuteDeleteAsync() > 0;
    }
}