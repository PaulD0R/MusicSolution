using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Repositories;
using UserService.Domain.Entities;
using UserService.Infrastructure.Contexts;

namespace UserService.Infrastructure.Repositories;

public class PersonRepository(
    UserManager<Person> userManager,
    AppDbContext context) 
    : IPersonRepository
{
    public async Task<Person?> GetByIdAsync(string id)
    {
        return await context.Persons.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Person?> GetByNameAsync(string name)
    {
        return await context.Persons.FirstOrDefaultAsync(x => x.UserName == name);
    }

    public async Task<ICollection<Person>> GetAllAsync()
    {
        return await userManager.Users.ToListAsync();
    }

    public async Task<bool> UpdateAsync(Person person)
    {
        var result =await context.Persons.Where(p => p.Id == person.Id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(p => p.UserName, n => person.UserName ?? n.UserName)
                    .SetProperty(p => p.Email, e => person.Email ?? e.Email)
                );

        return result != 0;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var person = await context.Persons.FirstOrDefaultAsync(x => x.Id == id);
        if (person == null) return false;

        var tokens = context.RefreshTokens.Where(x => x.PersonId == id);

        context.RefreshTokens.RemoveRange(tokens);
        context.Persons.Remove(person);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ChangeRoleAsync(string id, string newRole)
    {
        var person = await context.Persons.FirstOrDefaultAsync(x => x.Id == id);
            
        if (person == null) return false;

        await userManager.AddToRoleAsync(person, newRole);
        return true;
    }

    public async Task<bool> AddRoleByIdAsync(string id, string role)
    {
        var person = await context.Persons.FirstOrDefaultAsync(x => x.Id == id);
        if (person == null) return false;

        await userManager.AddToRoleAsync(person, role);

        return true;
    }

    public Task<bool> DeleteRoleByIdAsync(string id, string role)
    {
        throw new NotImplementedException();
    }
}