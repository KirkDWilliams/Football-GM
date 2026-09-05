using System.ComponentModel.DataAnnotations;
using FootballGm.Api.Data.Entity.Associations;

namespace FootballGm.Api.Data.Entity.Contrived;

public class Team
{
    [Key]
    public int TeamId { get; set; }
    public required string UserId { get; set; }
    public required int LeagueId { get; set; }

    [MaxLength(40)]
    public string Name { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public League League { get; set; } = null!;
    public ICollection<TeamPlayers> TeamPlayers { get; set; } = null!;
}
