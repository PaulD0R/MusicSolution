using System.ComponentModel.DataAnnotations;

namespace RoomService.Application.DTOs.Musics;

public record MusicActionRequest(
    [Required] [Length(5, 5)] string RoomId,
    [Required] Guid MusicId,
    [Required] int NewPosition,
    [Required] bool IsActive
    );