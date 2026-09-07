namespace FootballGm.Api.Data.Models;

public class DraftOutcome
{
    public required User User { get; set; }
    public required string TeamName { get; set; }
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The List of winning bids for a given player.
    /// </summary>
    public required List<Bid> DraftedPlayers { get; set; }
}
