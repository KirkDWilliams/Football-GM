using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public class StatScore
{
    public StatType StatType { get; init; }
    public float Value { get; init; }
}
