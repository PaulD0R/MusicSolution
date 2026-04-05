using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces.Messages;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Mappers;
using UserService.Application.Models.Person;
using UserService.Application.Models.Token;
using UserService.Domain.Entities;
using UserService.Domain.Events;
using UserService.Domain.Exceptions;

namespace UserService.Application.Services;

public class AuthenticationService(
    IPersonRepository personRepository,
    IJwtService jwtService,
    IRefreshTokenRepository refreshTokenRepository,
    //IMessageProducer<PersonCreateEvent> messageProducer,
    UserManager<Person> userManager,
    SignInManager<Person> signInManager,
    IHttpContextAccessor  httpContextAccessor,
    ILogger<AuthenticationService> logger)
    : IAuthenticationService
{
    public async Task<TokensDto> SigninAsync(SigninRequest signinRequest)
    {
        var person = await personRepository.GetByNameAsync(signinRequest.UserName!);
        if (person == null) throw new BadRequestException("Person with this name not found");
        
        var result = await signInManager.CheckPasswordSignInAsync(person, signinRequest.Password!, false);
        if (!result.Succeeded) throw new BadRequestException("Incorrect password");

        var token = await jwtService.CreateJwtAsync(person);
        var refreshToken = await refreshTokenRepository.CreateNewRefreshTokenAsync(person);

        var httpContext = httpContextAccessor.HttpContext;
        httpContext.Response.Cookies.Append("jwt", token);
        
        return new TokensDto
        {
            Jwt = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<TokensDto> SignupAsync(SignupRequest signupRequest)
    {
        var person = signupRequest.ToPerson();
        var createPerson = await userManager.CreateAsync(person, signupRequest.Password!);

        if (!createPerson.Succeeded)
            throw new UsernameAlreadyExistsException(person.UserName!);
        logger.LogInformation("Person {PersonUserName} registered successfully", person.UserName);    
        
        var roleResult = await userManager.AddToRoleAsync(person, "User");
        if (!roleResult.Succeeded)
            throw new Exception("failed to issue license");

        var token = await jwtService.CreateJwtAsync(person);
        var refreshToken = await refreshTokenRepository.CreateNewRefreshTokenAsync(person);
        logger.LogInformation($"Tokens was created successfully");

        //await messageProducer.ProduceAsync(person.ToPersonCreateEvent());
        //logger.LogInformation($"Kafka is working successfully");

        var httpContext = httpContextAccessor.HttpContext;
        httpContext?.Response.Cookies.Append("jwt", token);
        logger.LogInformation($"Cookies was created successfully");
        
        return new TokensDto 
        {
            Jwt = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<bool> LogoutAsync(string id)
    {
        return await refreshTokenRepository.DeleteRefreshTokensByUserIdAsync(id) ?
            true : throw new NotFoundException($"Person {id} not found");
    }
}