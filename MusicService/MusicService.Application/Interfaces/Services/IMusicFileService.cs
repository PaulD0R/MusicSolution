using Microsoft.AspNetCore.Http;

namespace MusicService.Application.Interfaces.Services;

public interface IMusicFileService
{
    Stream GetStreamByPath(string path);
    int GetBitrateByPath(string path);
    Task AddAsync(IFormFile file, string path);
    void DeleteAsync(string path);
}