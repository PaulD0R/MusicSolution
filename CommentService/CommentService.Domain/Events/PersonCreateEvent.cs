namespace CommentService.Domain.Events;

public record PersonCreateEvent
{
    public string PersonId { get; set; } = null!;
    public string Name { get; set; } = null!;
}