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

        public override string ToString()
        {
            return $"{HomeTeam.Name} - {HomeTeam.TeamId} | {AwayTeam.Name} - {AwayTeam.TeamId}";
        }

        public static Matchup FromEntity(Data.Entity.Contrived.Matchup entity) => new()
        {
            LeagueId = entity.LeagueId,
            MatchupId = entity.MatchupId,
            HomeTeam = entity.HomeTeam,
            AwayTeam = entity.AwayTeam,
            HomeScore = entity.HomeScore,
            AwayScore = entity.AwayScore,
        };

        public static List<Matchup> FromEntities(List<Data.Entity.Contrived.Matchup> entities)
            => entities.Select(entity => FromEntity(entity)).ToList();
    }
}
