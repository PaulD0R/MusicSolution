using CommentService.Domain.Events;
using CommentService.Domain.Models;

namespace CommentService.Application.Mappers;

public static class PersonMapper
{
    public static Person ToPerson(this PersonCreateEvent createEvent) =>
        new()
        {
            Id = createEvent.PersonId,
            Name = createEvent.Name
        };
    
    public static Person ToPerson(this PersonUpdateEvent updateEvent) =>
        new()
        {
            Id = updateEvent.PersonId,
            Name = updateEvent.NewName
        };
}