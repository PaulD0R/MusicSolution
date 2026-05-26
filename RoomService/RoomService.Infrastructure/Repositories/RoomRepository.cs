using Microsoft.EntityFrameworkCore;
using RoomService.Application.Interfaces.Repositories;
using RoomService.Domain.Models;
using RoomService.Infrastructure.Context;

namespace RoomService.Infrastructure.Repositories;

public class RoomRepository(AppDbContext context) : IRoomRepository
{
    public async Task<string?> GetPasswordAsync(string roomId)
    {
        return await context.Rooms
            .Where(r => r.Id == roomId)
            .Select(r => r.Password)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<string>> GetRoomIdsByPersonIdAsync(string personId)
    {
        return await context.Rooms
            .Where(r => r.Persons.Any(p => p.Id == personId))
            .Select(r => r.Id).ToListAsync();
    }

    public async Task<bool> AddPersonToRoomAsync(string roomId, Person person)
    {
        var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null) return false;
        if (await context.Rooms.AnyAsync(r => r.Id == roomId && r.Persons.Any(p => p.Id == person.Id)))
            return true; 

        room.Persons.Add(person);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Room?> AddRoomAsync(Room room)
    {
        var newRoom = (await context.Rooms.AddAsync(room)).Entity;
        return await context.SaveChangesAsync() > 0 ? newRoom : null;
    }

    public async Task<bool> RemoveRoomAsync(string roomId)
    {
        return await context.Rooms.Where(r => r.Id == roomId).ExecuteDeleteAsync() > 0;
    }

    public async Task<bool> UpdateRoomAsync(Room room)
    {
        context.Rooms.Update(room);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> IsAdminAsync(string roomId, string personId)
    {
        return await context.Rooms.AnyAsync(r => r.Id == roomId && r.AdminId == personId);
    }

    public async Task<bool> IsUserAsync(string roomId, string personId)
    {
        return await context.Rooms.AnyAsync(r => r.Id == roomId && r.Persons.Any(p => p.Id == personId));
    }

    public async Task<Room?> GetRoomByIdAsync(string roomId)
    {
        return await context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
    }
}