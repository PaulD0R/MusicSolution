using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MusicService.Application.Interfaces.Services;
using MusicService.Domain.Exceptions;

namespace MusicService.Application.Services;

public class MusicFileService(ILogger<MusicFileService> logger) : IMusicFileService
{
    public Stream GetStreamByPath(string path)
    {
        if (File.Exists(path))
            return new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 65536,
                useAsync: true);
        
        logger.LogError($"File {path} does not exist");
        throw new NotFoundException($"File not found by path {path}");
    }

    public int GetBitrateByPath(string path)
    {
        using var file = TagLib.File.Create(path);
        return file.Properties.AudioBitrate;
    }

    public async Task AddAsync(IFormFile file, string path)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".mp3", ".wav", ".flac", ".ogg" };

        if (!allowedExtensions.Contains(extension))
        {
            
            throw new BadRequestException("Invalid file type");
        }

        await using var stream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None, 
            65536, 
            useAsync: true);
        
        await file.CopyToAsync(stream);
        logger.LogInformation($"Adding file {path}");
    }

    public void DeleteAsync(string path)
    {
        File.Delete(path);
    }
}