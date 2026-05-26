using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RoomService.Application.DTOs.Musics;
using RoomService.Application.Interfaces.Services;

namespace RoomService.API.Controllers;

[Authorize]
public class RoomHub(IRoomService roomService, ILogger<RoomHub> logger) : Hub
{
    public async Task JoinRoom(string roomId)
    {
        var personId = Context.UserIdentifier;
        
        logger.LogInformation("Signal received: JoinRoom. User: {PersonId}, Room: {RoomId}", personId, roomId);

        if (personId == null) Context.Abort();
        else
        {
            var room = await roomService.GetRoomAsync(roomId, personId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId); 
            await Clients.Group(roomId).SendAsync("JoinRoom", room);
        }      
    }

    public async Task LeaveRoom(string roomId)
    {
        var personId = Context.UserIdentifier;

        logger.LogInformation("Signal received: LeaveRoom. User: {PersonId}, Room: {RoomId}", personId, roomId);

        if (personId == null) Context.Abort();
        else await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);      
    }

    public async Task SendAction(MusicActionRequest request)
    {
        var personId = Context.UserIdentifier;

        logger.LogInformation("Signal received: SendAction. User: {PersonId}, Room: {RoomId}, Track: {MusicId}", 
            personId, request.RoomId, request.MusicId);

        if (personId == null) Context.Abort();
        else
        {
            var music = await roomService.ChangeMusicDataAsync(request, personId);
            await Clients.Group(request.RoomId).SendAsync("SendAction", music);
        }
    } 
}