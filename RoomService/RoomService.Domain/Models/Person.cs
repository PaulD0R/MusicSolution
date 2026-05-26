namespace RoomService.Domain.Models;

public class Person
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<Room> Rooms { get; set; } = [];
}