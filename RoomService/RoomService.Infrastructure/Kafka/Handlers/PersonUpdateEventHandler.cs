using RoomService.Application.Interfaces.Factories;
using RoomService.Application.Interfaces.Messages;
using RoomService.Application.Interfaces.Services;
using RoomService.Domain.Events;

namespace RoomService.Infrastructure.Kafka.Handlers;

public class PersonUpdateEventHandler(IFactory<IPersonService> factory) : IMessageHandler<PersonUpdateEvent>
{
    public Task HandleAsync(PersonUpdateEvent message, CancellationToken cancellationToken)
    {
        var service = factory.Create();
        return service.UpdatePersonAsync(message);
    }
}