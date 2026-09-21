using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived
{
    public class Matchup
    {
        [Key]
        public int MatchupId { get; set; }
        public int LeagueId { get; set; }
        public int Week {  get; set; }

        public required Team HomeTeam { get; set; }
        public required Team AwayTeam { get; set; }

        public float HomeScore { get; set; }
        public float AwayScore { get; set; }
    }
}
