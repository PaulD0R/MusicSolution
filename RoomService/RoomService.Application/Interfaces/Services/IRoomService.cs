using RoomService.Application.DTOs.Musics;
using RoomService.Application.DTOs.Rooms;

namespace RoomService.Application.Interfaces.Services;

public interface IRoomService
{
    Task<RoomDto> AddRoomAsync(NewRoomRequest roomRequest, string personId);
    Task JoinToRoomAsync(JoinRoomRequest joinRequest, string personId);
    Task<MusicDto> ChangeMusicDataAsync(MusicActionRequest  musicActionRequest, string personId);
    Task RemoveAsync(string roomId, string personId);
    Task<RoomDto> GetRoomAsync(string roomId, string personId);
    Task<IEnumerable<string>> GetRoomIdsByPersonIdAsync(string personId);
}