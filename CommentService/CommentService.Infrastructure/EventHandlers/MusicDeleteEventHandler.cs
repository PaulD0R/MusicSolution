using CommentService.Application.Interfaces.Factories;
using CommentService.Application.Interfaces.Messages;
using CommentService.Application.Interfaces.Services;
using CommentService.Domain.Events;

namespace CommentService.Infrastructure.EventHandlers;

public class MusicDeleteEventHandler(IFactory<ICommentService> factory) : IMessageHandler<MusicDeleteEvent>
{
    public async Task HandleAsync(MusicDeleteEvent message, CancellationToken ct = default)
    {
        var service = factory.Create();
        await service.DeleteByMusicIdAsync(message);
    }
}