using RoomService.Application.DTOs.Musics;
using RoomService.Application.DTOs.Rooms;
using RoomService.Domain.Models;

namespace RoomService.Application.Mappers;

public static class RoomMapper
{
    extension(Room room)
    {
        public RoomDto ToRoomDto(IEnumerable<string> personNames) => new(
            room.Id,
            personNames,
            room.MusicId,
            room.IsActive,
            room.Position + (int)(DateTime.UtcNow - room.ActionTime).TotalSeconds
        );

        public MusicDto ToMusicDto() => new(
            room.MusicId,
            room.IsActive,
            room.Position);
    }
}