namespace FootballGm.Api.Data.Entity.Ingested;

public class PlayerGame
{
    public string PlayerId { get; init; } = string.Empty;
    public string GameId { get; init; } = string.Empty;

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
}
