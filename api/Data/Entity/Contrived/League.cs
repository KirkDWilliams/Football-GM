using FootballGm.Api.Data.Entity.Associations;
using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived
{
    public class League
    {
        [Key]
        public int LeagueId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<Team> Teams { get; set; } = [];
    }
}
