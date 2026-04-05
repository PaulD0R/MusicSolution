using UserService.Application.Models.Person;
using UserService.Domain.Entities;
using UserService.Domain.Events;

namespace UserService.Application.Mappers;

public static class PersonMapper
    {
        public static Person ToPerson(this SignupRequest signupRequest)
        {
            return new Person {
                UserName = signupRequest.UserName,
                Email = signupRequest.Email,
            };
        }

        public static Person ToPerson(this UpdatePersonRequest updatePersonRequest, string id)
        {
            return new Person
            {
                Id = id,
                UserName = updatePersonRequest.UserName,
                Email = updatePersonRequest.Email
            };
        }

        extension(Person person)
        {
            public PersonDto ToPersonDto()
            {
                return new PersonDto
                {
                    Id = person.Id,
                    UserName = person.UserName ??  string.Empty
                };
            }

            public PrivatePersonDto ToPrivatePersonDto()
            {
                return new PrivatePersonDto
                {
                    Id = person.Id,
                    UserName = person.UserName ??  string.Empty,
                    Email = person.Email ??  string.Empty
                };
            }

            public PersonCreateEvent ToPersonCreateEvent()
            {
                return new PersonCreateEvent
                {
                    PersonId = person.Id,
                    Name = person.UserName ?? string.Empty
                };
            }
            
            public PersonDeleteEvent ToPersonDeleteEvent()
            {
                return new PersonDeleteEvent
                {
                    PersonId = person.Id
                };
            }
            
            public PersonUpdateEvent ToPersonUpdateEvent()
            {
                return new PersonUpdateEvent
                {
                    PersonId = person.Id,
                    NewName = person.UserName ?? string.Empty
                };
            }
        }
    }