namespace RoomService.Application.DTOs.Rooms;

public record RoomDto(
    string Id,
    IEnumerable<string> PersonNames,
    Guid MusicId,
    bool IsActive,
    int Position
    );