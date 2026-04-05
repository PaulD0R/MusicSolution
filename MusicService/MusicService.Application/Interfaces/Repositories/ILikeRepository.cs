using MusicService.Domain.Models;

namespace MusicService.Application.Interfaces.Repositories;

public interface ILikeRepository
{
    public Task<bool> AddAsync(Like like);
    public Task<bool> DeleteAsync(Like like);
    public Task<IEnumerable<MusicData>> GetLikeMusicByUserIdAsync(string userId);
}