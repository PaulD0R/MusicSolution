namespace MusicService.Application.DTOs.Music;

public record MusicDto
{
    public Guid Id { get; set; }
    public int Bitrate { get; set; }
    public Stream Stream { get; set; } = null!;
}