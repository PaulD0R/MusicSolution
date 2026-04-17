using CommentService.Application.Interfaces.Services;
using CommentService.Domain.Events;
using MediatR;

namespace CommentService.Infrastructure.EventHandlers;

public class CommentDeleteEventHandler(ICommentFileService service) : INotificationHandler<CommentDeleteEvent>
{
    public async Task Handle(CommentDeleteEvent notification, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(notification.CommentId);
    }
}