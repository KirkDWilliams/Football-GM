using FootballGm.Api.Data.Entity.Associations;
using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public short Wins { get; set; }
        public short Loses { get; set; }
        public int LeagueId { get; set; }
        public League League { get; set; } = null!;
        public ICollection<TeamPlayers> TeamPlayers { get; set; } = new List<TeamPlayers>();

    }
}
