using FootballGm.Api.Data.Entity;
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

    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<PlayerGame> PlayerGameStats { get; set; }
    public DbSet<PlayerSeason> PlayerSeasonStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Single primary keys
        modelBuilder.Entity<Game>()
            .HasKey(g => g.GameId);

        modelBuilder.Entity<Player>()
            .HasKey(p => p.PlayerId);

        // Composite keys
        modelBuilder.Entity<PlayerGame>()
            .HasKey(pg => new { pg.PlayerId, pg.GameId });

        modelBuilder.Entity<PlayerSeason>()
            .HasKey(ps => new { ps.PlayerId, ps.Season });
    }
}
