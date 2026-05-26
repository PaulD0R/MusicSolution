using CommentService.Application.Interfaces.Factories;
using CommentService.Application.Interfaces.Messages;
using CommentService.Application.Interfaces.Services;
using CommentService.Domain.Events;

namespace CommentService.Infrastructure.EventHandlers;

public class PersonCreateEventHandler(IFactory<IPersonService> factory) : IMessageHandler<PersonCreateEvent>
{
    public async Task HandleAsync(PersonCreateEvent message, CancellationToken ct = default)
    {
        var service = factory.Create();
        await service.AddAsync(message);
    }
}