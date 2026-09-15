using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Infrastructure;

public interface IDraftRepository
{
    Task<Draft> AddAsync(Draft draft, CancellationToken cancellationToken = default);
    Task<Draft?> GetAsync(int leagueId, CancellationToken cancellationToken = default);
    Task<Draft?> GetLatestAsync(int leagueId, CancellationToken cancellationToken = default);
}

public class DraftRepository(AppDbContext context) : IDraftRepository
{
    public async Task<Draft> AddAsync(Draft draft, CancellationToken cancellationToken = default)
    {
        context.Drafts.Add(draft);
        await context.SaveChangesAsync(cancellationToken);
        return draft;
    }

    public async Task<Draft?> GetAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return await context.Drafts
            .FirstOrDefaultAsync(
                d => d.LeagueId == leagueId && d.Status != DraftStatus.Closed,
                cancellationToken);
    }

    public async Task<Draft?> GetLatestAsync(int leagueId, CancellationToken cancellationToken = default)
    {
        return await context.Drafts
            .Where(d => d.LeagueId == leagueId)
            .OrderByDescending(d => d.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
