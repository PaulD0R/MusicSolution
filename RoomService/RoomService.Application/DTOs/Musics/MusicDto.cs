namespace RoomService.Application.DTOs.Musics;

public record MusicDto(
    Guid Id,
    bool IsActive,
    int Position
    );