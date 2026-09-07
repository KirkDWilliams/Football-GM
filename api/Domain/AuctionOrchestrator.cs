using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure;
using FootballGm.Api.Infrastructure.Interfaces;
using static FootballGm.Api.Data.Models.Auction;

namespace FootballGm.Api.Domain;
public interface IAuctionOrchestrator
{
    /// <summary>
    /// Starts a new auction for a player in a league. Initializes all participants and sets the first bidder.
    /// </summary>
    Task<AuctionState> StartAuctionAsync(int leagueId, string playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Places a bid during an auction. Validates bid amount and transitions to next bidder if valid.
    /// </summary>
    Task<AuctionState> PlaceBidAsync(int leagueId, Data.Models.Bid bid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the current bidder as passed, transitioning to the next active participant.
    /// </summary>
    Task<AuctionState> PassAsync(int leagueId, string playerId, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current auction state.
    /// </summary>
    Task<AuctionState> GetAuctionStateAsync(int leagueId, string playerId, CancellationToken cancellationToken = default);
}

public class AuctionOrchestrator(IAuctionRepository auctionRepository, ILeagueRepository leagueRepository, IPlayerRepository playerRepository) : IAuctionOrchestrator
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly ILeagueRepository _leagueRepository = leagueRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;


    public async Task<AuctionState> StartAuctionAsync(int leagueId, string playerId, CancellationToken cancellationToken = default)
    {
        var league = await _leagueRepository.GetByIdAsync(leagueId, cancellationToken)
            ?? throw new Exception("League does not exist.");

        var memberIds = league.Members.Select(m => m.UserId).ToList();

        var existing = await _auctionRepository.GetAuctionAsync(leagueId, playerId, cancellationToken);

        if (existing != null)
            throw new InvalidOperationException($"Auction already exists for player {playerId}");

        var auction = new Data.Entity.Contrived.Auction
        {
            LeagueId = leagueId,
            PlayerId = playerId,
            CurrentBidderId = memberIds.First(), // First member goes first
            HighestBidderId = null,
            HighestBidRating = 0f,
            CurrentTurnStartedAtUtc = DateTimeOffset.UtcNow,
            Status = AuctionStatus.Active,
            AuctionMembers = memberIds.Select((id, index) =>
            new AuctionMember
            {
                UserId = id,
                TurnOrder = index,
                HasPassed = false
            }).ToList()
        };

        return await _auctionRepository.CreateAuctionAsync(auction, cancellationToken);
    }

    public async Task<AuctionState> GetAuctionStateAsync(int leagueId, string playerId, CancellationToken cancellationToken = default)
    {
        var league = await _leagueRepository.GetByIdAsync(leagueId, cancellationToken)
            ?? throw new InvalidOperationException("League not found");

        var auction = await _auctionRepository.GetAuctionAsync(leagueId, playerId, cancellationToken)
            ?? throw new InvalidOperationException("Auction not found");

        var player = await _playerRepository.GetPlayerByIdAsync(playerId, cancellationToken)
            ?? throw new InvalidOperationException("Player not found");

        var userIds = auction.AuctionMembers.Select(p => p.UserId).ToList();

        var userIdToName = league.Members.ToDictionary(u => u.UserId, u => u.User.DisplayName);
        return AuctionState.From(auction, player.Name, player.Position, player.Team, userIdToName);
    }

    public async Task<AuctionState> PassAsync(int leagueId, string playerId, string userId, CancellationToken cancellationToken = default)
    {
        var auction = await _auctionRepository.GetAuctionAsync(leagueId, playerId, cancellationToken)
            ?? throw new InvalidOperationException("Auction not found");

        if (auction.Status != AuctionStatus.Active)
            throw new InvalidOperationException("Auction is not active");

        if (auction.CurrentBidderId != userId)
            throw new InvalidOperationException("It is not this user's turn");

        var participant = auction.AuctionMembers.FirstOrDefault(p => p.UserId == userId)
            ?? throw new InvalidOperationException("User is not a participant");

        if (participant.HasPassed)
            throw new InvalidOperationException("User has already passed");

        participant.HasPassed = true;
        auction.CurrentTurnStartedAtUtc = DateTimeOffset.UtcNow;

        var nextBidderId = GetNextBidderId(auction);
        if (nextBidderId == null)
        {
            auction.Status = AuctionStatus.Completed;
        }
        else
        {
            auction.CurrentBidderId = nextBidderId;
        }

        await _auctionRepository.UpdateAuctionAsync(auction, cancellationToken);

        return await GetAuctionStateAsync(leagueId, playerId, cancellationToken);
    }

    public async Task<AuctionState> PlaceBidAsync(int leagueId, Data.Models.Bid bid, CancellationToken cancellationToken = default)
    {
        var auction = await _auctionRepository.GetAuctionAsync(leagueId, bid.PlayerId, cancellationToken)
            ?? throw new InvalidOperationException("Auction not found");

        if (auction.Status != AuctionStatus.Active)
            throw new InvalidOperationException("Auction is not active");

        if (auction.CurrentBidderId != bid.UserId)
            throw new InvalidOperationException("It is not this user's turn");

        var participant = auction.AuctionMembers.FirstOrDefault(p => p.UserId == bid.UserId)
           ?? throw new InvalidOperationException("User is not a participant in this auction");

        if (participant.HasPassed)
            throw new InvalidOperationException("This user has already passed");

        if (bid.Rating <= auction.HighestBidRating)
            throw new InvalidOperationException(
                $"Bid rating must exceed current bid of {auction.HighestBidRating}");

        var newHighestBid = new Data.Entity.Contrived.Bid
        {
            AuctionId = auction.AuctionId,
            PlayerId = bid.PlayerId,
            UserId = bid.UserId,
            Rating = bid.Rating
        };

        auction.Bids.Add(newHighestBid);
        auction.HighestBidRating = newHighestBid.Rating;
        auction.HighestBidderId = newHighestBid.UserId;
        auction.CurrentTurnStartedAtUtc = DateTimeOffset.UtcNow;

        var nextBidderId = GetNextBidderId(auction);
        if (nextBidderId == null)
        {
            auction.Status = AuctionStatus.Completed;
        }
        else
        {
            auction.CurrentBidderId = nextBidderId;
        }

        await _auctionRepository.UpdateAuctionAsync(auction, cancellationToken);

        return await GetAuctionStateAsync(leagueId, bid.PlayerId, cancellationToken);
    }

    private static string? GetNextBidderId(Data.Entity.Contrived.Auction auction)
    {
        var activeBidders = auction.AuctionMembers
            .Where(p => !p.HasPassed)
            .OrderBy(p => p.TurnOrder)
            .ToList();

        if (activeBidders.Count == 1)
            return null;

        var currentIndex = activeBidders.FindIndex(p => p.UserId == auction.CurrentBidderId);
        return activeBidders[(currentIndex + 1) % activeBidders.Count].UserId;
    }
}
