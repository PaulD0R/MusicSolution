namespace UserService.Application.Models.Person;

public record PersonDto
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
}