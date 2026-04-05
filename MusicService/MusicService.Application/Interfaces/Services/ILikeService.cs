using MusicService.Application.DTOs.Music;
using MusicService.Domain.Models;

namespace MusicService.Application.Interfaces.Services;

public interface ILikeService
{
    public Task AddAsync(Guid musicId, string userId);
    public Task DeleteAsync(Guid musicId, string userId);
    public Task<IEnumerable<MusicDataDto>> GetLikeMusicByUserIdAsync(string userId);
}