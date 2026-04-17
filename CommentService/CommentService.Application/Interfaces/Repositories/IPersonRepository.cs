using CommentService.Domain.Models;

namespace CommentService.Application.Interfaces.Repositories;

public interface IPersonRepository
{
    Task<bool> AddAsync(Person person);
    Task<bool> UpdateAsync(Person person);
    Task<bool> DeleteAsync(string id);
}