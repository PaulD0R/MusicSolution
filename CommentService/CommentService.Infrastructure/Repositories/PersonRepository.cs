using CommentService.Application.Interfaces.Repositories;
using CommentService.Domain.Models;
using CommentService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Infrastructure.Repositories;

public class PersonRepository(AppDbContext context) : IPersonRepository
{
    public async Task<bool> AddAsync(Person person)
    {
        await context.Persons.AddAsync(person);
        return await context.SaveChangesAsync() > 0;
    }

    public Task<bool> UpdateAsync(Person person)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await context.Persons.Where(p => p.Id == id).ExecuteDeleteAsync() > 0;
    }
}