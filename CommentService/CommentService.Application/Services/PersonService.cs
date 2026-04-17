using CommentService.Application.Interfaces.Repositories;
using CommentService.Application.Interfaces.Services;
using CommentService.Application.Mappers;
using CommentService.Domain.Events;
using CommentService.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace CommentService.Application.Services;

public class PersonService(
    IPersonRepository personRepository,
    ILogger<PersonService> logger)
    : IPersonService
{
    public async Task AddAsync(PersonCreateEvent createEvent)
    {
        if (!await personRepository.AddAsync(createEvent.ToPerson()))
            throw new InternalServerErrorException("Failed to create person");
        logger.LogInformation("Person {CreateEventId} created", createEvent.PersonId);
    }

    public Task UpdateAsync(PersonUpdateEvent updateEvent)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(PersonDeleteEvent deleteEvent)
    {        
        if (!await personRepository.DeleteAsync(deleteEvent.PersonId))
            throw new InternalServerErrorException("Failed to delete person");
        logger.LogInformation("Person {DeleteEventId} was deleted", deleteEvent.PersonId);
    }
}