using RoomService.Domain.Models;

namespace RoomService.Application.Interfaces.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetPersonByIdAsync(string personId);
    Task<IEnumerable<string>> GetPersonNamesByRoomIdAsync(string roomId);
    Task<bool> AddPersonAsync(Person person);
    Task<bool> RemovePersonAsync(string personId);
    Task<bool> UpdatePersonAsync(Person person);
}