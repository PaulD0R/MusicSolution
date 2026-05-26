namespace MusicService.Domain.Events;

public class MusicCreateEvent
{
    public Guid MusicId { get; set; }
    public string Name { get; set; } = null!;
}