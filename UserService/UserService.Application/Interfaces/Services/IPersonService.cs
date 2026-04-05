using UserService.Application.Models.Person;

namespace UserService.Application.Interfaces.Services;

public interface IPersonService
{
    Task<PersonDto> GetByIdAsync(string id);  
    Task<PrivatePersonDto> GetMeAsync(string id);
    Task<bool> UpdatePersonAsync(string id, UpdatePersonRequest personRequest);
    Task<bool> DeleteByIdAsync(string id);
}