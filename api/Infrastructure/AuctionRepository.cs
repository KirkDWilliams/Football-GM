using FootballGm.Api.Data.Entity.Contrived;
using static FootballGm.Api.Data.Models.Auction;

namespace FootballGm.Api.Infrastructure;

public interface IAuctionRepository
{
    Task<Auction> GetAuctionAsync(int leagueId, string playerId, CancellationToken cancellationToken);

    Task<AuctionState> CreateAuctionAsync(Auction auction, CancellationToken cancellationToken);

    Task UpdateAuctionAsync(Auction auction, CancellationToken cancellationToken);
}

public class AuctionRepository : IAuctionRepository
{
    public Task<Auction> GetAuctionAsync(int leagueId, string playerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<AuctionState> CreateAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
