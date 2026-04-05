using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MusicService.Application.DTOs.Helpers;
using MusicService.Application.DTOs.Music;
using MusicService.Application.Interfaces.Caching;
using MusicService.Application.Interfaces.Messages;
using MusicService.Application.Interfaces.Repositories;
using MusicService.Application.Interfaces.Services;
using MusicService.Application.Mappers;
using MusicService.Application.Options;
using MusicService.Domain.Events;
using MusicService.Domain.Exceptions;
using MusicService.Domain.Models;

namespace MusicService.Application.Services;

public class MusicService(
    IOptions<MusicOptions> options,
    IMusicDataRepository musicDataRepository,
    IMusicFileService musicFileService,
    //IMessageProducer<MusicCreateEvent> musicCreateEventProducer,
    //IMessageProducer<MusicDeleteEvent> musicDeleteEventProducer
    ICachingService cachingService)
    : IMusicService
{
    private const string MusicCacheKey = "music_";
    
    public async Task<IEnumerable<MusicDataDto>> GetAllAsync(MusicFindRequest findRequest)
    {
        var musics = await musicDataRepository.GetAllAsync(findRequest);
        return musics.Select(m => m.ToMusicDataDto());
    }

    public async Task<MusicDto> GetByIdAsync(Guid id)
    {
        var musicDto = await cachingService.GetAsync<MusicDto>(MusicCacheKey + id);
        if (musicDto != null) return musicDto;
        
        var musicData = await musicDataRepository.GetByIdAsync(id);
        if (musicData == null) throw new NotFoundException("Music not found");
            
        var stream = musicFileService.GetStreamByPath(Path.Combine(options.Value.StartPath, musicData.Path));
        musicDto = musicData.ToMusicDto(stream);
        await cachingService.SetAsync(MusicCacheKey + id, musicDto, TimeSpan.FromDays(1));

        return musicDto;
    }

    public async Task AddAsync(IFormFile file, string name)
    {
        var musicId = Guid.NewGuid();
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var filePath = $"{musicId}{extension}";
        var fileFullPath = Path.Combine(options.Value.StartPath, $"{musicId}{extension}");
        
        try
        {
            await musicFileService.AddAsync(file, fileFullPath);
        }
        catch (Exception ex)
        {
            throw new InternalServerErrorException(ex.Message);
        }
        
        var music = await musicDataRepository.AddAsync(new MusicData
        {
            Id = musicId,  
            Bitrate = musicFileService.GetBitrateByPath(fileFullPath),
            Name = name,
            Path = filePath
        }); 
        
        if (music == null)
        {
            musicFileService.DeleteAsync(fileFullPath);
            throw new InternalServerErrorException("Failed");
        }

        //await musicCreateEventProducer.ProduceAsync(music.ToMusicCreateEvent());
    }

    public async Task DeleteAsync(Guid id)
    {
        var musicData = await musicDataRepository.GetByIdAsync(id);
        if  (musicData == null)
            throw new NotFoundException("Music not found");
        if (!await musicDataRepository.DeleteAsync(id)) 
            throw new InternalServerErrorException("Failed");

        await cachingService.RemoveAsync(MusicCacheKey + id);
        
        try
        {
            musicFileService.DeleteAsync(Path.Combine(options.Value.StartPath, musicData.Path));
        }
        catch (Exception ex)
        {
            await musicDataRepository.AddAsync(musicData);
            throw new InternalServerErrorException(ex.Message);
        }

        //await musicDeleteEventProducer.ProduceAsync(new MusicDeleteEvent { Id = id });
    }
}