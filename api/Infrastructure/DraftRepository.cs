using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public interface IDraftRepository
{
    Task<Draft> AddAsync(Draft draft, CancellationToken cancellationToken = default);
    Task<Draft> UpdateAsync(Draft draft, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the league's in-play draft. Closed rows are excluded, so a league
    /// whose only draft is closed returns null. The table allows one in-play row
    /// per league.
    /// </summary>
    Task<Draft?> GetAsync(int leagueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the newest draft row for the league. A closed draft counts.
    /// </summary>
    Task<Draft?> GetLatestAsync(int leagueId, CancellationToken cancellationToken = default);
}

public class DraftRepository(AppDbContext context) : IDraftRepository
{
    public async Task<Draft> AddAsync(Draft draft, CancellationToken cancellationToken = default)
    {
        context.Drafts.Add(draft);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsInPlayConflict(exception))
        {
            context.Entry(draft).State = EntityState.Detached;
            throw new DraftAlreadyInPlayException();
        }

        return draft;
    }

    public async Task<Draft> UpdateAsync(Draft draft, CancellationToken cancellationToken = default)
    {
        context.Drafts.Update(draft);
        await context.SaveChangesAsync(cancellationToken);
        return draft;
    }

    public async Task<Draft?> GetAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return await context.Drafts
            .Where(d => d.LeagueId == leagueId && d.Status != DraftStatus.Closed)
            .OrderByDescending(d => d.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Draft?> GetLatestAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return await context.Drafts
            .Where(d => d.LeagueId == leagueId)
            .OrderByDescending(d => d.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static bool IsInPlayConflict(DbUpdateException exception)
    {
        for (Exception? inner = exception; inner != null; inner = inner.InnerException)
        {
            if (inner is SqliteException sqlite &&
                sqlite.Message.Contains(
                    "UNIQUE constraint failed: Drafts.LeagueId",
                    StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}

/// <summary>
/// Another in-play draft for the league won the insert. Closed drafts do not count.
/// </summary>
public sealed class DraftAlreadyInPlayException : Exception;
