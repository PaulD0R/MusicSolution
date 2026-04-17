namespace CommentService.Domain.Events;

public record PersonUpdateEvent
{
    public string PersonId { get; set; } = null!;
    public string NewName { get; set; } = null!;
}