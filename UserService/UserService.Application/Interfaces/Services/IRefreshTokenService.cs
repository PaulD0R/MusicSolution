using UserService.Application.Models.Token;

namespace UserService.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<TokensDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);   
}