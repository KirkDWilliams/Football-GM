using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Models;

public class Bid
{
    public required string PlayerId { get; set; }
    public required int AuctionId { get; init; }

    [MaxLength(32)]
    public required string UserId { get; init; }


    /// <summary>
    /// The bid preference rating.
    /// </summary>
    public required float Rating { get; init; }

    public required int Duration { get; set; }
    public required float Salary { get; set; }
    public float SigningBonus { get; set; } = 0f;
}
