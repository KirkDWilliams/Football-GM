using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public class StatScore
{
    public StatType StatType { get; init; }
    public decimal Value { get; init; }
}
