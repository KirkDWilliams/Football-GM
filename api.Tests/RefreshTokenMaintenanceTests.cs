using FootballGm.Api.Auth;
using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity;
using FootballGm.Api.Data.Entity.Contrived;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballGm.Api.Tests;

public sealed class RefreshTokenMaintenanceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly RefreshTokenMaintenance _maintenance;

    public RefreshTokenMaintenanceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
        _db.Users.Add(new User
        {
            Id = "user-1",
            Email = "nick@example.com",
            DisplayName = "Nick",
            PasswordHash = "hash",
            CreatedAtUtc = DateTimeOffset.UtcNow
        });
        _db.SaveChanges();

        _maintenance = new RefreshTokenMaintenance(
            _db,
            Options.Create(new JwtOptions { RefreshTokenCleanupRetentionDays = 7 }));
    }

    [Fact]
    public async Task Cleanup_deletes_expired_tokens()
    {
        SeedToken("expired", expiresAt: DateTimeOffset.UtcNow.AddDays(-1));

        var removed = await _maintenance.CleanupAsync();

        Assert.Equal(1, removed);
        Assert.Empty(_db.RefreshTokens.AsNoTracking().ToList());
    }

    [Fact]
    public async Task Cleanup_keeps_active_tokens()
    {
        SeedToken("active", expiresAt: DateTimeOffset.UtcNow.AddDays(10));

        var removed = await _maintenance.CleanupAsync();

        Assert.Equal(0, removed);
        Assert.Single(_db.RefreshTokens.AsNoTracking().ToList());
    }

    [Fact]
    public async Task Cleanup_deletes_revoked_tokens_past_retention()
    {
        SeedToken(
            "revoked-old",
            expiresAt: DateTimeOffset.UtcNow.AddDays(10),
            revokedAt: DateTimeOffset.UtcNow.AddDays(-8));

        var removed = await _maintenance.CleanupAsync();

        Assert.Equal(1, removed);
        Assert.Empty(_db.RefreshTokens.AsNoTracking().ToList());
    }

    [Fact]
    public async Task Cleanup_keeps_revoked_tokens_inside_retention()
    {
        SeedToken(
            "revoked-new",
            expiresAt: DateTimeOffset.UtcNow.AddDays(10),
            revokedAt: DateTimeOffset.UtcNow.AddDays(-2));

        var removed = await _maintenance.CleanupAsync();

        Assert.Equal(0, removed);
        Assert.Single(_db.RefreshTokens.AsNoTracking().ToList());
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private void SeedToken(string id, DateTimeOffset expiresAt, DateTimeOffset? revokedAt = null)
    {
        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = id,
            UserId = "user-1",
            TokenHash = $"hash-{id}",
            CreatedAtUtc = DateTimeOffset.UtcNow.AddDays(-20),
            ExpiresAtUtc = expiresAt,
            RevokedAtUtc = revokedAt
        });
        _db.SaveChanges();
    }
}
