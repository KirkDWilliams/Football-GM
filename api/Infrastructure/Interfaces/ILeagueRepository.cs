using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Infrastructure.Interfaces;

public interface ILeagueRepository
{
    Task<League> AddAsync(League league, CancellationToken cancellationToken = default);
}
