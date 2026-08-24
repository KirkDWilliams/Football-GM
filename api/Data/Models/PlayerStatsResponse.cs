namespace FootballGm.Api.Data.Models;

public class PlayerStatsResponse
{
    public string PlayerId { get; init; } = string.Empty;
    public string GameId { get; init; } = string.Empty;

    public decimal PlayerPerformance { get; init; } = decimal.Zero;

    public short PassAttempts { get; init; }
    public short PassCompletions { get; init; }
    public short PassingYards { get; init; }
    public short PassingTouchdowns { get; init; }

    public short RushAttempts { get; init; }
    public short RushingYards { get; init; }
    public short RushingFirstDowns { get; init; }
    public short RushingTouchdowns { get; init; }

    public short Receptions { get; init; }
    public short ReceivingYards { get; init; }
    public short ReceivingTouchdowns { get; init; }


    public short Interceptions { get; init; }
    public short Fumbles { get; init; }
    public short Sacks { get; init; }

    public string FieldGoalsMade { get; init; } = string.Empty;
    public string FieldGoalsMissed { get; init; } = string.Empty;
    public short ExtraPointsMade { get; init; }
    public short ExtraPointsAttempted { get; init; }
    public short PassingTwoPointConversions { get; init; }
    public short RushingTwoPointConversions { get; init; }
    public short ReceivingTwoPointConversions { get; init; }
    public short ReturnedTouchdowns { get; init; }

    public PlayerStatsResponse(Entity.Ingested.PlayerSeason seasonStats)
    {
        PlayerId = seasonStats.PlayerId;
        GameId = string.Empty; // season-level stats have no single GameId

        //PlayerPerformance = CalculatePlayerSeaonsPerformance();

        PassAttempts = seasonStats.PassAttempts;
        PassCompletions = seasonStats.PassCompletions;
        PassingYards = seasonStats.PassingYards;
        PassingTouchdowns = seasonStats.PassingTouchdowns;

        RushAttempts = seasonStats.RushAttempts;
        RushingYards = seasonStats.RushingYards;
        RushingFirstDowns = seasonStats.RushingFirstDowns;
        RushingTouchdowns = seasonStats.RushingTouchdowns;

        Receptions = seasonStats.Receptions;
        ReceivingYards = seasonStats.ReceivingYards;
        ReceivingTouchdowns = seasonStats.ReceivingTouchdowns;

        Interceptions = seasonStats.Interceptions;
        Fumbles = seasonStats.Fumbles;
        Sacks = seasonStats.Sacks;

        FieldGoalsMade = seasonStats.FieldGoalsMade;
        FieldGoalsMissed = seasonStats.FieldGoalsMissed;
        ExtraPointsMade = seasonStats.ExtraPointsMade;
        ExtraPointsAttempted = seasonStats.ExtraPointsAttempted;
        PassingTwoPointConversions = seasonStats.PassingTwoPointConversions;
        RushingTwoPointConversions = seasonStats.RushingTwoPointConversions;
        ReceivingTwoPointConversions = seasonStats.ReceivingTwoPointConversions;
        ReturnedTouchdowns = seasonStats.ReturnedTouchdowns;
    }

    public PlayerStatsResponse(Entity.Ingested.PlayerGame gameStats)
    {
        PlayerId = gameStats.PlayerId;
        GameId = gameStats.GameId; // season-level stats have no single GameId

        //PlayerPerformance = CalculatePlayerGamePerformance();

        PassAttempts = gameStats.PassAttempts;
        PassCompletions = gameStats.PassCompletions;
        PassingYards = gameStats.PassingYards;
        PassingTouchdowns = gameStats.PassingTouchdowns;

        RushAttempts = gameStats.RushAttempts;
        RushingYards = gameStats.RushingYards;
        RushingFirstDowns = gameStats.RushingFirstDowns;
        RushingTouchdowns = gameStats.RushingTouchdowns;

        Receptions = gameStats.Receptions;
        ReceivingYards = gameStats.ReceivingYards;
        ReceivingTouchdowns = gameStats.ReceivingTouchdowns;

        Interceptions = gameStats.Interceptions;
        Fumbles = gameStats.Fumbles;
        Sacks = gameStats.Sacks;

        FieldGoalsMade = gameStats.FieldGoalsMade;
        FieldGoalsMissed = gameStats.FieldGoalsMissed;
        ExtraPointsMade = gameStats.ExtraPointsMade;
        ExtraPointsAttempted = gameStats.ExtraPointsAttempted;
        PassingTwoPointConversions = gameStats.PassingTwoPointConversions;
        RushingTwoPointConversions = gameStats.RushingTwoPointConversions;
        ReceivingTwoPointConversions = gameStats.ReceivingTwoPointConversions;
        ReturnedTouchdowns = gameStats.ReturnedTouchdowns;
    }


}
