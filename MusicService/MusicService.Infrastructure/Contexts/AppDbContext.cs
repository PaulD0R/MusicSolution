using Microsoft.EntityFrameworkCore;
using MusicService.Domain.Models;

namespace MusicService.Infrastructure.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MusicData>  MusicData { get; set; }
    public DbSet<Like> Likes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.Entity<MusicData>(entity =>
        {
            entity.HasIndex(e => e.Name)
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops"); 
        });
        
        modelBuilder.Entity<Like>().HasKey(l => new { l.MusicId, l.UserId });

        base.OnModelCreating(modelBuilder);
    }
}