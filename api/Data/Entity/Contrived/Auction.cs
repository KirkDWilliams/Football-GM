using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived;

[Index(nameof(LeagueId), nameof(PlayerId), IsUnique = true)]
public class Auction
{
    [Key]
    public int AuctionId { get; init; }
    public required int LeagueId { get; init; }
    public required string PlayerId { get; init; }

    /// <summary>
    /// The user whose turn it currently is in the auction.
    /// </summary>
    public required string CurrentBidderId { get; set; }

    /// <summary>
    /// The user ID of who placed the current highest bid.
    /// </summary>
    public string? HighestBidderId { get; set; }

    /// <summary>
    /// The current highest bid amount.
    /// </summary>
    public float HighestBidRating { get; set; } = 0f;


    /// <summary>
    /// When the current turn started.
    /// </summary>
    public DateTimeOffset CurrentTurnStartedAtUtc { get; set; }

    /// <summary>
    /// Status of the auction (Active, Completed, Cancelled).
    /// </summary>
    public AuctionStatus Status { get; set; } = AuctionStatus.Active;

    /// <summary>
    /// List of Members still in the auction.
    /// </summary>
    public List<AuctionMember> AuctionMembers { get; set; } = [];

    // Navigation Properties
    public League League { get; init; } = null!;
    public ICollection<Bid> Bids { get; init; } = [];
    public ICollection<List<User>> LeagueMembers { get; init; } = [];
}

public enum AuctionStatus
{
    Active = 0,
    Completed = 1,
    Cancelled = 2
}
