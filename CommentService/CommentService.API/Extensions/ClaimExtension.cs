using System.Security.Claims;

namespace CommentService.API.Extensions;

public static class ClaimExtension
{
    public static string? GetUserId(this ClaimsPrincipal principal) =>
        principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
}