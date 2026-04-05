using MusicService.Application.DTOs.Music;
using MusicService.Application.Interfaces.Repositories;
using MusicService.Application.Interfaces.Services;
using MusicService.Application.Mappers;
using MusicService.Domain.Exceptions;
using MusicService.Domain.Models;

namespace MusicService.Application.Services;

public class LikeService(ILikeRepository likeRepository) : ILikeService
{
    public async Task AddAsync(Guid musicId, string userId)
    {
        if (!await likeRepository.AddAsync(new Like { MusicId = musicId, UserId = userId }))
            throw new InternalServerErrorException("Can't add like");

    }

    public async Task DeleteAsync(Guid musicId, string userId)
    {
        if (!await likeRepository.DeleteAsync(new Like { MusicId = musicId, UserId = userId }))
            throw new InternalServerErrorException("Can't delete like");
    }

    public async Task<IEnumerable<MusicDataDto>> GetLikeMusicByUserIdAsync(string userId)
    {
        var music = await likeRepository.GetLikeMusicByUserIdAsync(userId);
        return music.Select(m => m.ToMusicDataDto());
    }
}