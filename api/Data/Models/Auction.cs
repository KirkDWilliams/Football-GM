using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public class Auction
{
    /// <summary>
    /// Current state of an active auction.
    /// </summary>
    public class AuctionState
    {
        public required int LeagueId { get; init; }
        public required int AuctionId { get; init; }
        public required string PlayerId { get; init; }
        public required string PlayerName { get; init; }
        public required string PlayerPosition { get; init; }
        public required string PlayerTeam { get; init; }

        /// <summary>
        /// The current highest bid rating.
        /// </summary>
        public required float HighestBidRating { get; init; }

        /// <summary>
        /// The user ID whose turn it is.
        /// </summary>
        public required string CurrentBidderId { get; init; }

        /// <summary>
        /// The name of the user whose turn it is.
        /// </summary>
        public required string CurrentBidderName { get; init; }

        /// <summary>
        /// List of all bids placed so far.
        /// </summary>
        public required List<BidRecord> BidHistory { get; init; }

        /// <summary>
        /// Participants still active in the auction.
        /// </summary>
        public required List<AuctionParticipantInfo> ActiveParticipants { get; init; }

        /// <summary>
        /// Participants who have passed.
        /// </summary>
        public required List<AuctionParticipantInfo> PassedParticipants { get; init; }

        /// <summary>
        /// The user who placed the highest bid.
        /// </summary>
        public string? HighestBidderId { get; init; }

        public static AuctionState From(
            Data.Entity.Contrived.Auction auction,
            string playerName,
            string playerPosition,
            string playerTeam,
            Dictionary<string, string> userIdToName)
        {
            var bidHistory = auction.Bids
                .OrderBy(b => b.Rating)
                .Select(b => new BidRecord
                {
                    BidId = b.BidId,
                    UserId = b.UserId,
                    UserName = userIdToName.GetValueOrDefault(b.UserId, b.UserId),
                    Rating = b.Rating,
                })
                .ToList();

            var activeParticipants = auction.AuctionMembers
                .Where(p => !p.HasPassed)
                .OrderBy(p => p.TurnOrder)
                .Select(p => new AuctionParticipantInfo
                {
                    UserId = p.UserId,
                    UserName = userIdToName.GetValueOrDefault(p.UserId, p.UserId),
                    TurnOrder = p.TurnOrder
                })
                .ToList();

            var passedParticipants = auction.AuctionMembers
                .Where(p => p.HasPassed)
                .Select(p => new AuctionParticipantInfo
                {
                    UserId = p.UserId,
                    UserName = userIdToName.GetValueOrDefault(p.UserId, p.UserId),
                    TurnOrder = p.TurnOrder
                })
                .ToList();

            return new AuctionState
            {
                AuctionId = auction.AuctionId,
                LeagueId = auction.LeagueId,
                PlayerId = auction.PlayerId,
                PlayerName = playerName,
                PlayerPosition = playerPosition,
                PlayerTeam = playerTeam,
                HighestBidRating = auction.HighestBidRating,
                CurrentBidderId = auction.CurrentBidderId,
                CurrentBidderName = userIdToName.GetValueOrDefault(auction.CurrentBidderId, auction.CurrentBidderId),
                BidHistory = bidHistory,
                ActiveParticipants = activeParticipants,
                PassedParticipants = passedParticipants,
                HighestBidderId = auction.HighestBidderId
            };
        }

        public class BidRecord
        {
            public required int BidId { get; init; }
            public required string UserId { get; init; }
            public required string UserName { get; init; }
            public required float Rating { get; init; }
        }

        public class AuctionParticipantInfo
        {
            public required string UserId { get; init; }
            public required string UserName { get; init; }
            public required int TurnOrder { get; init; }
        }
    }
}
