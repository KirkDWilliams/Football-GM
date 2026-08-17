namespace FootballGm.Api.Data.Models;

public class WeeklyBudget
{
    public decimal BaseWeeklyCapSpace { get; set; }
    public decimal RollOverCapSpace { get; set; } = decimal.Zero;
    public decimal FutureObligations { get; set; } = decimal.Zero;
    public decimal CurrentObligations { get; set; } = decimal.Zero;
}
