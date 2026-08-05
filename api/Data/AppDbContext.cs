using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Data;

/// <summary>
/// Application database context. Domain entities and DbSets will be added as features are built.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
