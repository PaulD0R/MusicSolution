using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options): IdentityDbContext<Person>(options)
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {   
        base.OnModelCreating(builder);
        
        const string ADMIN_ID = "B9C6AA99-E98D-4E3D-87DE-C93E17592919";
        const string USER_ID = "19EF95CD-413A-49EA-B4F1-448E9D86D81C";

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        List<IdentityRole> roles =
        [
            new()
            {   
                Id = Guid.Parse(ADMIN_ID).ToString(),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.Parse(ADMIN_ID).ToString()
            },

            new()
            {
                Id = Guid.Parse(USER_ID).ToString(),
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.Parse(USER_ID).ToString()
            }
        ];

        builder.Entity<IdentityRole>().HasData(roles);
    }
}