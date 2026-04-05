using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Services;

public interface IJwtService
{
    Task<string> CreateJwtAsync(Person person);
}