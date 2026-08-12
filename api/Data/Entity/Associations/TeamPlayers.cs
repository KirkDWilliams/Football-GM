using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Entity.Ingested;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballGm.Api.Data.Entity.Associations
{
    public class TeamPlayers
    {
        [ForeignKey(nameof(Team))]
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        [ForeignKey(nameof(Player))]
        public string PlayerId { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
