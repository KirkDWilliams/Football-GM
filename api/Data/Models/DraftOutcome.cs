namespace FootballGm.Api.Data.Models;

public class DraftOutcome
{
    public required User User { get; set; }
    public required string TeamName { get; set; }
    public string Description { get; set; } = string.Empty;
    public required Dictionary<string, Contract> DraftedPlayers { get; set; }
}
