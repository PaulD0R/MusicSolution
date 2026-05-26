using RoomService.Domain.Events;
using RoomService.Domain.Models;

namespace RoomService.Application.Interfaces.Services;

public interface IPersonService
{
    Task AddPersonAsync(PersonCreateEvent createEvent);
    Task RemovePersonAsync(PersonDeleteEvent deleteEvent);
    Task UpdatePersonAsync(PersonUpdateEvent updateEvent);
    Task<Person> GetPersonByIdAsync(string id);
    Task<IEnumerable<string>> GetPersonNamesByRoomIdAsync(string roomId);
}