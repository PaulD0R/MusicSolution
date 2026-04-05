using Microsoft.AspNetCore.Http;
using MusicService.Application.DTOs;
using MusicService.Application.DTOs.Helpers;
using MusicService.Application.DTOs.Music;

namespace MusicService.Application.Interfaces.Services;

public interface IMusicService
{
    Task<IEnumerable<MusicDataDto>> GetAllAsync(MusicFindRequest findRequest);
    Task<MusicDto> GetByIdAsync(Guid id);
    Task AddAsync(IFormFile file, string name);
    Task DeleteAsync(Guid id);
}