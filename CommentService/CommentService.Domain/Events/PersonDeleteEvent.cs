namespace CommentService.Domain.Events;

public record PersonDeleteEvent
{
    public string PersonId { get; set; } = null!;
}