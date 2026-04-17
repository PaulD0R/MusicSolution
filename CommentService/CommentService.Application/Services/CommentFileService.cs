using CommentService.Application.Interfaces.Caches;
using CommentService.Application.Interfaces.Services;
using CommentService.Application.Options;
using CommentService.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xabe.FFmpeg;

namespace CommentService.Application.Services;

public class CommentFileService(
    IOptions<CommentFileOptions> options,
    ICachingService cachingService,
    ILogger<CommentFileService> logger)
    : ICommentFileService
{
    private const string VideoKey = "video_";
    private const string PictureKey = "picture_";

    public async ValueTask<Stream> GetStreamByIdAsync(Guid id)
    {
        var stream = await cachingService.GetAsync<FileStream>($"{VideoKey}{id}");
        if (stream != null) return stream;
        
        var fullPath = Path.Combine(options.Value.VideoStartPath, $"{id}.mp4");
        if (File.Exists(fullPath))
        {
            stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 65536,
                useAsync: true);
            
            await cachingService.SetAsync($"{VideoKey}{id}", stream, TimeSpan.FromDays(1));
            return stream;
        }

        logger.LogError("File {FullPath} does not exist", fullPath);
        throw new NotFoundException($"File not found by path {fullPath}");
    }
    
    public async Task<Dictionary<Guid, byte[]>> GetPicturesByIdsAsync(IEnumerable<Guid> ids)
    {
        var tasks = ids.Select(async id =>
        {
            var data = await cachingService.GetAsync<byte[]>($"{PictureKey}{id}");
            if (data != null) return new { Id = id, Data = data };
            
            var path = Path.Combine(options.Value.PictureStartPath, $"{id}.jpg");
            if (!File.Exists(path))
                return new { Id = id, Data = Array.Empty<byte>() };

            data = await File.ReadAllBytesAsync(path);
            await cachingService.SetAsync($"{PictureKey}{id}", data, TimeSpan.FromDays(1));
            return new { Id = id, Data = data };
        });

        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(x => x.Id, x => x.Data);
    }

    public async Task AddAsync(IFormFile file, Guid id)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (extension != ".mp4")
            throw new BadRequestException("Invalid file type");

        var videoPath = Path.Combine(options.Value.VideoStartPath, $"{id}.mp4");
        var picturePath = Path.Combine(options.Value.PictureStartPath, $"{id}.jpg");
    
        {
            await using var stream = new FileStream(
                videoPath,
                FileMode.Create,
                FileAccess.Write,   
                FileShare.None, 
                65536, 
                useAsync: true);
        
            await file.CopyToAsync(stream);
            await stream.FlushAsync();
        }

        logger.LogInformation("Adding file {FullPath}", videoPath);
    
        try 
        {
            await FFmpeg.Conversions.New()
                .AddParameter($"-i \"{videoPath}\"")
                .AddParameter("-ss 00:00:01")
                .AddParameter("-vframes 1") 
                .SetOutput(picturePath)
                .Start();

            logger.LogInformation("Adding file {FullPath}", picturePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "FFmpeg failed for {VideoPath}", videoPath);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        File.Delete(Path.Combine(options.Value.VideoStartPath, $"{id}.mp4"));
        File.Delete(Path.Combine(options.Value.PictureStartPath, $"{id}.jpg"));
        await cachingService.RemoveAsync($"{VideoKey}{id}");
        await cachingService.RemoveAsync($"{PictureKey}{id}");
    }

    public async Task DeleteRangeAsync(IEnumerable<Guid> ids)
    {
        var files = Directory.GetFiles(options.Value.PictureStartPath)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(Guid.Parse)
            .ToList();

        var existingIds = ids.Where(i => files.Contains(i));
        var orphans = files.Except(existingIds);

        foreach (var id in orphans)
        {
            File.Delete(Path.Combine(options.Value.VideoStartPath, $"{id}.mp4"));
            File.Delete(Path.Combine(options.Value.PictureStartPath, $"{id}.jpg"));
            await cachingService.RemoveAsync($"{VideoKey}{id}");
            await cachingService.RemoveAsync($"{PictureKey}{id}");
        }
    }
}