using Microsoft.EntityFrameworkCore;
using RoomService.Domain.Models;

namespace RoomService.Infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Person> Persons { get; set; }
}