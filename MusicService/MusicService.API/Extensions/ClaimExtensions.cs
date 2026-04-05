using System.Security.Claims;

namespace MusicService.API.Extensions;

public static class ClaimExtensions
{
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
}