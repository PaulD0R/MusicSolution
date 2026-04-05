using UserService.Application.Models.Person;
using UserService.Application.Models.Token;

namespace UserService.Application.Interfaces.Services;

public interface IAuthenticationService
{
    Task<TokensDto> SigninAsync(SigninRequest signinRequest);
    Task<TokensDto> SignupAsync(SignupRequest signupRequest);
    Task<bool> LogoutAsync(string id);
}