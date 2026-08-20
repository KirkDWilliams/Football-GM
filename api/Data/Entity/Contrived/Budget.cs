namespace FootballGm.Api.Data.Entity.Contrived;

public class Budget
{
    public int TeamId { get; set; }

    // a new week begins at midnight of the first game of that week (typically Thursday). 
    public int Week {  get; set; }

    public decimal CurrentObligations {  get; set; } = decimal.Zero;
}
