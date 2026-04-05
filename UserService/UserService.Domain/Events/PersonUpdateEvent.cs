namespace UserService.Domain.Events;

public class PersonUpdateEvent
{
    public string PersonId { get; set; } = null!;
    public string NewName { get; set; } = null!;
}