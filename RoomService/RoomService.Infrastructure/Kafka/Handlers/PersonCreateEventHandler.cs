using RoomService.Application.Interfaces.Factories;
using RoomService.Application.Interfaces.Messages;
using RoomService.Application.Interfaces.Services;
using RoomService.Domain.Events;

namespace RoomService.Infrastructure.Kafka.Handlers;

public class PersonCreateEventHandler(IFactory<IPersonService> factory) : IMessageHandler<PersonCreateEvent>
{
    public async Task HandleAsync(PersonCreateEvent message, CancellationToken cancellationToken)
    {
        var service = factory.Create();
        await service.AddPersonAsync(message);
    }
}