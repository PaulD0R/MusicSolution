using MusicService.Application.DTOs;
using MusicService.Application.DTOs.Music;
using MusicService.Domain.Events;
using MusicService.Domain.Models;

namespace MusicService.Application.Mappers;

public static class MusicMapper
{
        extension(MusicData musicData)
        {
            public MusicDataDto ToMusicDataDto() => new()
            {
                Id = musicData.Id,
                Name = musicData.Name
            };

            public MusicDto ToMusicDto(Stream stream) => new()
            {
                Id = musicData.Id,
                Bitrate = musicData.Bitrate,
                Stream = stream
            };

            public MusicCreateEvent ToMusicCreateEvent() => new()
            {
                MusicId = musicData.Id,
                Name = musicData.Name
            };
        }
}