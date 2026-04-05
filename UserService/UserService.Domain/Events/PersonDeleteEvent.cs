namespace UserService.Domain.Events;

public class PersonDeleteEvent
{
    public string PersonId { get; set; } = null!;
}