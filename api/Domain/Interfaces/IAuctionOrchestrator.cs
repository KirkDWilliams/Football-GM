using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Domain.Interfaces;

public interface IAuctionOrchestrator
{
    /// <summary>
    /// Starts a new auction for a player in a league. Initializes all participants and sets the first bidder.
    /// </summary>
    Task<Auction.AuctionState> StartAuctionAsync(int leagueId, string playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Places a bid during an auction. Validates bid amount and transitions to next bidder if valid.
    /// </summary>
    Task<Auction.AuctionState> PlaceBidAsync(int leagueId, Bid bid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the current bidder as passed, transitioning to the next active participant.
    /// </summary>
    Task<Auction.AuctionState> PassAsync(int leagueId, string playerId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current auction state.
    /// </summary>
    Task<Auction.AuctionState> GetAuctionStateAsync(int leagueId, string playerId, CancellationToken cancellationToken = default);
}
