using System.Security.Claims;

namespace UserService.API.Extensions;

public static class ClaimExtension
{
    public static string? GetId(this ClaimsPrincipal principal)
    {
        return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}   