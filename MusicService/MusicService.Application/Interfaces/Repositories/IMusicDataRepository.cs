using MusicService.Application.DTOs.Helpers;
using MusicService.Domain.Models;

namespace MusicService.Application.Interfaces.Repositories;

public interface IMusicDataRepository
{
    Task<IEnumerable<MusicData>> GetAllAsync(MusicFindRequest findRequest);
    Task<MusicData?> GetByIdAsync(Guid id);
    Task<MusicData?> AddAsync(MusicData musicData);
    Task<bool> DeleteAsync(Guid id);
}