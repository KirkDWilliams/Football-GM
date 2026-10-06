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

            foreach (var team in league.Teams)
            {
                var teamScore = await teamOrchestrator.CalculateTeamScore(week, team.TeamPlayers);

                // gather players
                // funnel them through rule-point matrix
                // sum the amount of points 

                var matchupForTeam = matchups.Find(mu => mu.HomeTeam.TeamId == team.TeamId ||
                                                            mu.AwayTeam.TeamId == team.TeamId);

                ArgumentException.ThrowIfNullOrEmpty(matchupForTeam?.ToString(), nameof(matchupForTeam));

                if (matchupForTeam.AwayTeam.TeamId == team.TeamId)
                    matchupForTeam.AwayScore = teamScore;

                if (matchupForTeam.HomeTeam.TeamId == team.TeamId)
                    matchupForTeam.HomeScore = teamScore;
            }

            return Matchup.FromEntities(matchups);
        }

        // Question: How do we not duplicate the calculation of a team's player's score? It seems like we would be doing this very often under our given
        // Question: the matchup screen showing all of the scores for the team's in the league would require a whole host of calculations.
        // Solution: perhaps we save columns of weekly scores (wk1, wk2, wk3, ..., wk15) that we are modifying during the current week. 
    }
}
