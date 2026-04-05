namespace MusicService.Domain.Models;

public class MusicData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Bitrate { get; set; }
    public string Path { get; set; } = null!;
    public ICollection<Like> Likes { get; set; } = [];
}