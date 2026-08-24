using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived;

public class League
{
    [Key]
    public int LeagueId { get; init; }

    [MaxLength(32)]
    public required string AdminUserId { get; init; }

    public int AdminTeamId { get; init; }

    [MaxLength(50)]
    public required string Name { get; init; }

    public required Settings Settings { get; init; }
    public ICollection<Team> Teams { get; init; } = [];
}
