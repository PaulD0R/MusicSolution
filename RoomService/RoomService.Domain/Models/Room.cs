using System.ComponentModel.DataAnnotations;

namespace RoomService.Domain.Models;

public class Room   
{
    [StringLength(5)] public string Id { get; set; } = null!;
    public string AdminId { get; set; } = null!;
    public ICollection<Person> Persons { get; set; } = [];
    public string Password { get; set; } = null!;
    public Guid MusicId { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActionTime { get; set; }
    public int Position { get; set; }
}