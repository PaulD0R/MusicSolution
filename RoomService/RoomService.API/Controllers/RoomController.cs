using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RoomService.API.Extensions;
using RoomService.Application.DTOs.Rooms;
using RoomService.Application.Interfaces.Services;

namespace RoomService.API.Controllers;

[ApiController]
[Authorize]
[Route("rooms")]
public class RoomController(IRoomService roomService) : ControllerBase
{
    [HttpPost("new-room")]
    public async Task<IActionResult> AddRooms(NewRoomRequest request)
    {
        var personId = User.GetUserId();
        if (personId == null) return Unauthorized();
        
        var room = await roomService.AddRoomAsync(request, personId);
        return Created($"rooms/{room.Id}", room);
    }

    [HttpPost]
    public async Task<IActionResult> JoinToRoom(JoinRoomRequest request)
    {           
        var personId = User.GetUserId();
        if (personId == null) return Unauthorized();
        
        await roomService.JoinToRoomAsync(request, personId);
        return Ok();
    }

    [HttpDelete("{id:length(5)}")]
    public async Task<IActionResult> DeleteRoom([FromRoute] string id, [FromServices] IHubContext<RoomHub> hubContext)
    {
        var personId = User.GetUserId();
        if (personId == null) return Unauthorized();
        
        await roomService.RemoveAsync(id, personId);
        await hubContext.Clients.Group(id).SendAsync("CloseRoom");
        
        return NoContent();
    }

    [HttpGet("persons/{personId:guid}")]
    public async Task<IActionResult> GetRoomsIds([FromRoute] string personId)
    {
        return Ok(await roomService.GetRoomIdsByPersonIdAsync(personId));
    }
}