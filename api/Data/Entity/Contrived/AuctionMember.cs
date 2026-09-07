using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived;

public class AuctionMember
{
    public int AuctionId { get; init; }

    [MaxLength(32)]
    public required string UserId { get; init; }

    /// <summary>
    /// Whether this participant has passed on this auction.
    /// </summary>
    public bool HasPassed { get; set; } = false;

    /// <summary>
    /// Turn order for round-robin style bidding.
    /// </summary>
    public int TurnOrder { get; init; }

    // Navigation Properties
    public Auction Auction { get; init; } = null!;
    public User User { get; init; } = null!;
}
