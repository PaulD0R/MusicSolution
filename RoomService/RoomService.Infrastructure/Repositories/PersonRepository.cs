using Microsoft.EntityFrameworkCore;
using RoomService.Application.Interfaces.Repositories;
using RoomService.Domain.Models;
using RoomService.Infrastructure.Context;

namespace RoomService.Infrastructure.Repositories;

public class PersonRepository(AppDbContext context) : IPersonRepository
{
    public async Task<Person?> GetPersonByIdAsync(string personId)
    {
        return await context.Persons.FirstOrDefaultAsync(p => p.Id == personId);
    }

    public async Task<IEnumerable<string>> GetPersonNamesByRoomIdAsync(string roomId)
    {
        return await context.Persons.Where(p => p.Rooms.Any(r => r.Id == roomId))
            .Select(p => p.Name).ToListAsync();
    }

    public async Task<bool> AddPersonAsync(Person person)
    {
        await context.Persons.AddAsync(person);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemovePersonAsync(string personId)
    {
        return await context.Persons.Where(p => p.Id == personId).ExecuteDeleteAsync() > 0;
    }

    public async Task<bool> UpdatePersonAsync(Person person)
    {
        context.Update(person);
        return await context.SaveChangesAsync() > 0;
    }
}