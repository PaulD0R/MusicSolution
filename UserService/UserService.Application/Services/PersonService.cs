using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces.Cachings;
using UserService.Application.Interfaces.Messages;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Mappers;
using UserService.Application.Models.Person;
using UserService.Domain.Events;
using UserService.Domain.Exceptions;

namespace UserService.Application.Services;

public class PersonService(
    IPersonRepository personRepository,
    //IMessageProducer<PersonDeleteEvent>  personDeleteProducer,
    //IMessageProducer<PersonUpdateEvent>  personUpdateProducer,
    IHashCachingService cachingService,
    ILogger<PersonService> logger)
    : IPersonService
{
    private const string KeyStart = "user:";
    
    public async Task<PersonDto> GetByIdAsync(string id)
    {
        var key = KeyStart + id;
        var personDto = await cachingService.GetAsync<PersonDto>(key);
        if (personDto != null) return personDto;
            
        var person = await personRepository.GetByIdAsync(id);
        if (person == null) throw new NotFoundException($"Person {id} not found");
                
        await cachingService.SetAsync(key, person.ToPrivatePersonDto());
        
        return person.ToPersonDto();
    }

    public async Task<PrivatePersonDto> GetMeAsync(string id)
    {
        var key = KeyStart + id;
        var personDto = await cachingService.GetAsync<PrivatePersonDto>(key);
        if (personDto != null) return personDto;
            
        var person = await personRepository.GetByIdAsync(id);
        if (person == null) throw new NotFoundException($"Person {id} not found");
                
        await cachingService.SetAsync(key, person.ToPrivatePersonDto());
                
        return person.ToPrivatePersonDto();
    }

    public async Task<bool> UpdatePersonAsync(string id, UpdatePersonRequest personRequest)
    {
        var person = personRequest.ToPerson(id);
        var result = await personRepository.UpdateAsync(person) ? true : 
            throw new NotFoundException($"Person {id} not found");

        if (!string.IsNullOrEmpty(person.Email)) 
            await cachingService.SetFieldAsync(KeyStart + id, nameof(person.Email), person.Email);
        if (string.IsNullOrEmpty(person.UserName)) return result;
        
        await cachingService.SetFieldAsync(KeyStart + id, nameof(person.UserName), person.UserName);
        //await personUpdateProducer.ProduceAsync(person.ToPersonUpdateEvent());
        logger.LogInformation("Name was changed successfully");

        return result;
    }

    public async Task<bool> DeleteByIdAsync(string id)
    {
        var result = await personRepository.DeleteUserAsync(id) ? true :
            throw new NotFoundException($"Person {id} not found");

        await cachingService.RemoveAsync(KeyStart + id);
        //await personDeleteProducer.ProduceAsync(new PersonDeleteEvent { PersonId = id });
        logger.LogInformation("Person {Id} was deleted successfully", id);
        
        return result;
    }
}