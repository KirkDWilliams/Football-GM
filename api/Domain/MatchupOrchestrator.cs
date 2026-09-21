using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain.Helpers;
using FootballGm.Api.Domain.Interfaces;
using FootballGm.Api.Infrastructure.Interfaces;
using static FootballGm.Api.Infrastructure.LeagueQueryExtensions;

namespace FootballGm.Api.Domain
{
    public class MatchupOrchestrator(
        ILeagueRepository leagueRepository,
        IMatchupRepository repository,
        ITeamOrchestrator teamOrchestrator) : IMatchupOrchestrator
    {
        public async Task<List<Matchup>?> GetMatchups(int leagueId, int week, CancellationToken cancellationToken)
        {
            var league = await leagueRepository.GetByIdAsync(
                leagueId,
                LeagueIncludes.Teams | LeagueIncludes.Settings,
                cancellationToken)
                ?? throw new InvalidOperationException($"Nothing found for the given League {leagueId}");

            var matchups = await repository.GetWeeklyMatchupsByLeagueId(leagueId, week, cancellationToken);

            // Calculate scores if current, else return stored off values
            if (WeekHelper.CurrentWeek == week)
            {
                foreach (var team in league.Teams)
                {
                    var teamScore = teamOrchestrator.CalculateTeamScore();
                    var matchupForTeam = matchups.Find(mu => mu.HomeTeam.TeamId == team.TeamId ||
                                                             mu.AwayTeam.TeamId == team.TeamId);

                    ArgumentException.ThrowIfNullOrEmpty(matchupForTeam?.ToString(), nameof(matchupForTeam));

                    if (matchupForTeam.AwayTeam.TeamId == team.TeamId)
                        matchupForTeam.AwayScore = teamScore.Result;

                    if (matchupForTeam.HomeTeam.TeamId == team.TeamId)
                        matchupForTeam.HomeScore = teamScore.Result;
                }
            }

            return Matchup.FromEntities(matchups);
        }
    }
}
