namespace UserService.Domain.Events;

public class PersonCreateEvent
{
    public string PersonId { get; set; } = null!;
    public string Name { get; set; } = null!;
}