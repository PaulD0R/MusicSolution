using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Interfaces.Services;
using UserService.Application.Options;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

public class JwtService(
    UserManager<Person> userManager,
    IOptions<JwtOptions> options) : IJwtService
{
    public async Task<string> CreateJwtAsync(Person person)
    {
        var roles = await userManager.GetRolesAsync(person);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, person.Id),
            new(JwtRegisteredClaimNames.GivenName, person.UserName ?? string.Empty)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwt = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: new SigningCredentials
                (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Secret!)),
                    SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}