using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Extensions;
using UserService.Application.Interfaces.Services;
using UserService.Application.Models.Person;

namespace UserService.API.Controllers;

[ApiController]
    [Authorize]
    [Route("persons")]
    public class PersonController(IPersonService  personService) : ControllerBase
    {
        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetPersonById([FromRoute] string id)
        {
            return Ok(await personService.GetByIdAsync(id));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetPrivatePerson()
        {
            var personId = User.GetId();
            if (personId == null) return Unauthorized("Не авторизирован");

            return Ok(await personService.GetMeAsync(personId));
        }

        [HttpPatch("me")]
        public async Task<IActionResult> UpdatePrivatePerson([FromBody] UpdatePersonRequest request)
        {
            var personId = User.GetId();
            if (personId == null) return Unauthorized();
            
            await personService.UpdatePersonAsync(personId,  request);
            
            return NoContent();
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeletePerson()
        {
            var personId =  User.GetId();
            if (personId == null) return Unauthorized("Не авторизирован");
            
            await personService.DeleteByIdAsync(personId);
            
            return NoContent();
        }
    }