using RoomService.Domain.Events;
using RoomService.Domain.Models;

namespace RoomService.Application.Mappers;

public static class PersonMapper
{
    public static Person ToPerson(this PersonCreateEvent createEvent) => new()
    {
        Id = createEvent.PersonId,
        Name = createEvent.Name
    };
}