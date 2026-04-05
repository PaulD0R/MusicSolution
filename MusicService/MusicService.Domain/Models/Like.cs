namespace MusicService.Domain.Models;

public class Like
{
    public Guid MusicId { get; set; }
    public MusicData? MusicData { get; set; }
    public string UserId { get; set; } = null!;
}