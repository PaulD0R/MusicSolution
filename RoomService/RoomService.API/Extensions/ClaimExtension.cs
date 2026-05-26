using System.Security.Claims;

namespace RoomService.API.Extensions;

public static class ClaimExtension
{
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
}