using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain.Interfaces;

namespace FootballGm.Api.Domain
{
    public class MatchupOrchestrator : IMatchupOrchestrator
    {
        public List<Matchup> GetMatchups(int league, int week)
        {
            // 1. Fetch league to ensure it exists
            // 2. Query matchups from repository
            // 3. Calculate scores if needed
            // 4. Return matchups
            return [];
        }
    }
}
