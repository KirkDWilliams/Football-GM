using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Tests;

public class DraftRepositoryTests
{
    [Fact]
    public async Task Add_rejects_a_second_in_play_draft_for_the_league()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var repository = new DraftRepository(context);

        await repository.AddAsync(new Draft { LeagueId = 7, Status = DraftStatus.Lobby });
        await repository.AddAsync(new Draft { LeagueId = 7, Status = DraftStatus.Closed });
        await repository.AddAsync(new Draft { LeagueId = 8, Status = DraftStatus.Live });

        await Assert.ThrowsAsync<DraftAlreadyInPlayException>(() =>
            repository.AddAsync(new Draft { LeagueId = 7, Status = DraftStatus.Live }));

        var inPlay = await repository.GetAsync(7);
        Assert.Equal(DraftStatus.Lobby, inPlay!.Status);
        Assert.Equal(2, await context.Drafts.CountAsync(draft => draft.LeagueId == 7));
    }
}
