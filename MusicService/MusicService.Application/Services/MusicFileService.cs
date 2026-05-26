using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicService.Application.Interfaces.Services;
using MusicService.Application.Options;
using MusicService.Domain.Exceptions;

namespace MusicService.Application.Services;

public class MusicFileService(
    IOptions<MusicFileOptions> options,
    ILogger<MusicFileService> logger)
    : IMusicFileService
{
    public Stream GetStreamByPath(string path)
    {
        var fullPath = Path.Combine(options.Value.StartPath, path);
        if (File.Exists(fullPath))
            return new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 65536,
                useAsync: true);
        
        logger.LogError("File {FullPath} does not exist", fullPath);
        throw new NotFoundException($"File not found by path {fullPath}");
    }

    public int GetBitrateByPath(string path)
    {
        using var file = TagLib.File.Create(Path.Combine(options.Value.StartPath, path));
        return file.Properties.AudioBitrate;
    }

    public async Task AddAsync(IFormFile file, string path)
    {
        var fullPath = Path.Combine(options.Value.StartPath, path);
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".mp3", ".wav", ".flac", ".ogg" };

        if (!allowedExtensions.Contains(extension))
            throw new BadRequestException("Invalid file type");

        await using var stream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,   
            FileShare.None, 
            65536, 
            useAsync: true);
        
        await file.CopyToAsync(stream);
        logger.LogInformation("Adding file {FullPath}", fullPath);
    }

    public void Delete(string path)
    {
        File.Delete(Path.Combine(options.Value.StartPath, path));
    }
}