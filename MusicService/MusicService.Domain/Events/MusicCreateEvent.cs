namespace MusicService.Domain.Events;

public class MusicCreateEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}