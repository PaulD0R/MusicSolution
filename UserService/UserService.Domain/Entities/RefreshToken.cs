using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities;

public class RefreshToken
{
    [MaxLength(38)] public string Id { get; set; } = null!;
    [MaxLength(500)] public string Token { get; set; } = null!;
    public DateTime LiveTime { get; set; }
    [MaxLength(38)] public string PersonId { get; set; } =  null!;
    public Person Person { get; set; } =   null!;
}