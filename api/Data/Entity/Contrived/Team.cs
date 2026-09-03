using System.ComponentModel.DataAnnotations;
using FootballGm.Api.Data.Entity.Associations;
using FootballGm.Api.Services;

namespace FootballGm.Api.Data.Entity.Contrived;

public class Team
{
    [Key] public int TeamId { get; set; }
    public required UserDto User { get; set; }
    public int LeagueId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public short Wins { get; set; } = 0;
    public short Loses { get; set; } = 0;

    public List<TeamPlayers> ActivePlayers { get; set; } = [];
    public List<TeamPlayers> InactivePlayers { get; set; } = [];

    public League League { get; set; } = null!;
    public ICollection<TeamPlayers> TeamPlayers { get; set; } = null!;
}
