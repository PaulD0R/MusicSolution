using CommentService.Domain.Events;

namespace CommentService.Application.Interfaces.Services;

public interface IPersonService
{
    Task AddAsync(PersonCreateEvent createEvent);
    Task UpdateAsync(PersonUpdateEvent updateEvent);
    Task DeleteAsync(PersonDeleteEvent deleteEvent);
}