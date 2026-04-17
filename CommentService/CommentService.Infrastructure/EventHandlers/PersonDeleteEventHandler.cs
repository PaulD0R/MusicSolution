using CommentService.Application.Interfaces.Factories;
using CommentService.Application.Interfaces.Messages;
using CommentService.Application.Interfaces.Services;
using CommentService.Domain.Events;

namespace CommentService.Infrastructure.EventHandlers;

public class PersonDeleteEventHandler(IFactory<IPersonService> factory) : IMessageHandler<PersonDeleteEvent>
{
    public async Task HandleAsync(PersonDeleteEvent message, CancellationToken ct = default)
    {
        var service = factory.Create();
        await service.DeleteAsync(message);
    }
}