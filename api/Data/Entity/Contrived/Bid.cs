using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Contrived;

public class Bid
{
    [Key]
    public int BidId { get; init; }
    public required string PlayerId { get; set; }
    public required int AuctionId { get; init; }

    [MaxLength(32)]
    public required string UserId { get; init; }

    /// <summary>
    /// The bid preference rating.
    /// </summary>
    public required float Rating { get; init; }


    // Navigation Properties
    public Auction Auction { get; init; } = null!;
    public User User { get; init; } = null!;
}
