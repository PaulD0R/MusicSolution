using System.ComponentModel.DataAnnotations;

namespace RoomService.Application.DTOs.Rooms;

public record NewRoomRequest(
    [Required] string Password
    );