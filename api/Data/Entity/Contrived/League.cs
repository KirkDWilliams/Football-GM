using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived;

public class League
{
    [Key] public int LeagueId { get; set; }
    public int AdminTeamId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal WeeklyCapSpace { get; set; } = 100M;

    public List<Rules> LeagueRules { get; set; } = []; // default rules
    public ICollection<Team> Teams { get; set; } = [];
}
