using CommentService.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Person> Persons { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(d => d.Parent)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade); 
        });
        
        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var entitiesWithEvents = ChangeTracker.Entries<Comment>()
            .Select(e => e.Entity)
            .Where(e => e.DeleteEvents.Count != 0)
            .ToList();

        var events = entitiesWithEvents.SelectMany(e => e.DeleteEvents).ToList();

        var result = await base.SaveChangesAsync(ct);

        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, ct);

        entitiesWithEvents.ForEach(e => e.DeleteEvents.Clear());

        return result;
    }
}