using System.ComponentModel.DataAnnotations;

namespace RoomService.Application.DTOs.Rooms;

public record JoinRoomRequest(
    [Required] [Length(5, 5)] string RoomId,
    [Required] string Password
    );