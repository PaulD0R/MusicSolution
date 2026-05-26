using RoomService.Domain.Models;

namespace RoomService.Application.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<string?> GetPasswordAsync(string roomId);
    Task<IEnumerable<string>> GetRoomIdsByPersonIdAsync(string personId);
    Task<bool> AddPersonToRoomAsync(string roomId, Person person);
    Task<Room?> AddRoomAsync(Room room);
    Task<bool> RemoveRoomAsync(string roomId);
    Task<bool> UpdateRoomAsync(Room room);
    Task<bool> IsAdminAsync(string roomId, string personId);
    Task<bool> IsUserAsync(string roomId, string personId);
    Task<Room?> GetRoomByIdAsync(string roomId);
}