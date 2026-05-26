using RoomService.Application.Interfaces.Repositories;
using RoomService.Application.Interfaces.Services;
using RoomService.Application.Mappers;
using RoomService.Domain.Events;
using RoomService.Domain.Exceptions;
using RoomService.Domain.Models;

namespace RoomService.Application.Services;

public class PersonService(IPersonRepository personRepository) : IPersonService
{
    public async Task AddPersonAsync(PersonCreateEvent createEvent)
    {
        if (!await personRepository.AddPersonAsync(createEvent.ToPerson()))     
            throw new InternalServerException("Person could not be added");
    }

    public async Task RemovePersonAsync(PersonDeleteEvent deleteEvent)
    {
        if (await personRepository.GetPersonByIdAsync(deleteEvent.PersonId) == null)
            throw new NotFoundException("Person not found");
        
        if (await personRepository.RemovePersonAsync(deleteEvent.PersonId))
            throw new InternalServerException("Person could not be removed");
    }

    public async Task UpdatePersonAsync(PersonUpdateEvent updateEvent)
    {
        var person = await personRepository.GetPersonByIdAsync(updateEvent.PersonId);
        if (person == null)
            throw new NotFoundException("Person not found");

        person.Name = updateEvent.NewName;
        if (!await personRepository.UpdatePersonAsync(person))
            throw new InternalServerException("Person could not be updated");
    }

    public async Task<Person> GetPersonByIdAsync(string id)
    {
        return await personRepository.GetPersonByIdAsync(id) 
               ??  throw new NotFoundException("Person not found");
    }

    public async Task<IEnumerable<string>> GetPersonNamesByRoomIdAsync(string roomId)
    {
        return await personRepository.GetPersonNamesByRoomIdAsync(roomId);
    }
}