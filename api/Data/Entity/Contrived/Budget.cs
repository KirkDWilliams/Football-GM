namespace FootballGm.Api.Data.Entity.Contrived;

public class Budget
{
    public int LeagueId { get; set; }
    public int TeamId { get; set; }

    // a new week begins at midnight of the first game of that week (typically Thursday). 
    public required decimal[] PaymentSchedule { get; set; }
}
