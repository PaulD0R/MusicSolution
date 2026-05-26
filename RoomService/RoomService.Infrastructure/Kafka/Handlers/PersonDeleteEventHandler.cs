using RoomService.Application.Interfaces.Factories;
using RoomService.Application.Interfaces.Messages;
using RoomService.Application.Interfaces.Services;
using RoomService.Domain.Events;

namespace RoomService.Infrastructure.Kafka.Handlers;

public class PersonDeleteEventHandler(IFactory<IPersonService> factory) : IMessageHandler<PersonDeleteEvent> 
{
    public async Task HandleAsync(PersonDeleteEvent message, CancellationToken cancellationToken)
    {
        var service = factory.Create();
        await service.RemovePersonAsync(message);
    }
}