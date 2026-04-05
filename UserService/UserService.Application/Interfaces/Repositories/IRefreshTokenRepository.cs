using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<string> CreateNewRefreshTokenAsync(Person person);
    Task<bool> DeleteRefreshTokensByUserIdAsync(string personId);
    Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);
    Task<RefreshToken> UpdateRefreshToken(RefreshToken refreshToken);
}