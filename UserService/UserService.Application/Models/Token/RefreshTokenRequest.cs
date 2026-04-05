using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Models.Token;

public record RefreshTokenRequest
{
    [Required]
    public string? Token { get; set; }
}