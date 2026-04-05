using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(string id);
    Task<Person?> GetByNameAsync(string name);
    Task<ICollection<Person>> GetAllAsync();
    Task<bool> UpdateAsync(Person person);
    Task<bool> DeleteUserAsync(string id);
    Task<bool> ChangeRoleAsync(string id, string newRole);
    Task<bool> AddRoleByIdAsync(string id, string role);
    Task<bool> DeleteRoleByIdAsync(string id, string role);
}