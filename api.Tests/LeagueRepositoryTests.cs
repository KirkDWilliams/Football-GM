using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Tests;

public class LeagueRepositoryTests
{
    [Fact]
    public async Task ListMembers_orders_by_join_time_on_sqlite()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.Users.AddRange(User("later", "Later"), User("earlier", "Earlier"));
        var league = new League
        {
            JoinCode = "CODE0001",
            Name = "Sunday",
            Settings = new Settings { EligiblePositions = [], Rules = [] },
            Members =
            [
                Member("later", new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero)),
                Member("earlier", new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero))
            ]
        };
        context.Leagues.Add(league);
        await context.SaveChangesAsync();

        var members = await new LeagueRepository(context).ListMembersAsync(league.LeagueId);

        Assert.Equal(["earlier", "later"], members.Select(member => member.UserId));
    }

    private static User User(string id, string name) => new()
    {
        Id = id,
        Email = $"{id}@example.com",
        DisplayName = name,
        PasswordHash = "hash",
        CreatedAtUtc = DateTimeOffset.UtcNow
    };

    private static LeagueMember Member(string userId, DateTimeOffset joinedAtUtc) => new()
    {
        UserId = userId,
        Role = LeagueMemberRole.Member,
        JoinedAtUtc = joinedAtUtc
    };
}
