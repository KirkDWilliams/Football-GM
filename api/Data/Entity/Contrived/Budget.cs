namespace FootballGm.Api.Data.Entity.Contrived;

public class Budget
{
    public int TeamId { get; set; }
    public int Week {  get; set; }

    public decimal FutureObligations { get; set; } = decimal.Zero;

    public decimal PreviousCapSurplus { get; set; } = decimal.Zero;
}
