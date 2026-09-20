using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Data.Models
{
    public class Matchup
    {
        public int LeagueId { get; set; }

        public int MatchupId { get; set; }

        public required Team HomeTeam { get; set; }

        public required Team AwayTeam { get; set; }

        public float HomeScore { get; set; }
        public float AwayScore { get; set; }
    }
}
