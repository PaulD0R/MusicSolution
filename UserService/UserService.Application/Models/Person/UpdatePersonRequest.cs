using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Models.Person;

public record UpdatePersonRequest(
    string? UserName = null,
    [EmailAddress] string? Email = null);