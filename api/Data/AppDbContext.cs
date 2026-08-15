using FootballGm.Api.Data.Entity;
using FootballGm.Api.Data.Entity.Associations;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Entity.Ingested;
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

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<PlayerGame> PlayerGame { get; set; }
    public DbSet<PlayerSeason> PlayerSeason { get; set; }
    public DbSet<InjuryStatus> InjuryStatus { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamPlayers> TeamPlayers { get; set; }
    public DbSet<Contract> Contracts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.DisplayName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.TokenHash).IsUnique();
            entity.HasIndex(t => t.UserId);
            entity.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(t => t.DeviceName).HasMaxLength(200);

            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Team>()
            .HasOne(t => t.League)
            .WithMany(l => l.Teams)
            .HasForeignKey(t => t.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamPlayers>()
            .HasKey(tp => new
            {
                tp.TeamId,
                tp.PlayerId,
                tp.ContractId
            });

        modelBuilder.Entity<TeamPlayers>()
            .HasOne(tp => tp.Team)
            .WithMany(t => t.TeamPlayers)
            .HasForeignKey(tp => tp.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamPlayers>()
            .HasOne(tp => tp.Player)
            .WithMany(p => p.TeamPlayers)
            .HasForeignKey(tp => tp.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamPlayers>()
            .HasOne(tp => tp.Contract)
            .WithMany(c => c.TeamPlayers)
            .HasForeignKey(tp => tp.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        // Single primary keys
        modelBuilder.Entity<Game>()
            .HasKey(g => g.GameId);

        modelBuilder.Entity<Player>()
            .HasKey(p => p.PlayerId);

        modelBuilder.Entity<Contract>()
            .HasKey(c => c.ContractId);

        // Composite keys
        modelBuilder.Entity<PlayerGame>()
            .HasKey(pg => new { pg.PlayerId, pg.GameId });

        modelBuilder.Entity<PlayerSeason>()
            .HasKey(ps => new { ps.PlayerId, ps.Season });

        modelBuilder.Entity<InjuryStatus>()
            .HasKey(i => new { i.Season, i.Week, i.PlayerId });
    }
}
