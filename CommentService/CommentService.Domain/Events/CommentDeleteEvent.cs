using MediatR;

namespace CommentService.Domain.Events;

public class CommentDeleteEvent : INotification
{
    public Guid CommentId { get; set; }
}