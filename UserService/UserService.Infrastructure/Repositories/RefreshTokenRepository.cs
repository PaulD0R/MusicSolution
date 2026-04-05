using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Repositories;
using UserService.Domain.Entities;
using UserService.Infrastructure.Contexts;

namespace UserService.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task<string> CreateNewRefreshTokenAsync(Person person)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid().ToString(),
            PersonId = person.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            LiveTime = DateTime.UtcNow.AddDays(7)
        };
            
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken.Token;
    }

    public async Task<bool> DeleteRefreshTokensByUserIdAsync(string personId)
    {
        return await context.RefreshTokens.Where(x => x.PersonId == personId)
            .ExecuteDeleteAsync() > 0;
    }

    public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
    {
        return await context.RefreshTokens.Include(x => x.Person)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task<RefreshToken> UpdateRefreshToken(RefreshToken refreshToken)
    {
        refreshToken.LiveTime = DateTime.UtcNow.AddDays(7);
        refreshToken.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        await context.SaveChangesAsync();
            
        return refreshToken;
    }
}