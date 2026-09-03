using FootballGm.Api.Services;

namespace FootballGm.Api.Data.Models;

public class DraftOutcome
{
    public required UserDto User { get; set; }
    public required string TeamName { get; set; }
    public required Dictionary<string, Contract> DraftedPlayers { get; set; }
}
