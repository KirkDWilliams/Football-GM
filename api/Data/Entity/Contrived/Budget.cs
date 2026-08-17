namespace FootballGm.Api.Data.Entity.Contrived;

public class Budget
{
    public int TeamId { get; set; }

    // a new week begins at midnight of the first game of that week (typically Thursday). 
    public int Week {  get; set; }


    // if the current week is 7 and a trade occurs, the dead cap needs to be applied to the following week.
    public decimal FutureObligations { get; set; } = decimal.Zero;

    // this should represent the dead cap paid this week already. Obligations should come from a budget once the week switches.
    public decimal CurrentObligations {  get; set; } = decimal.Zero;

    // this is the roll-over surplus from not spending up to the cap last year.
    public decimal PreviousCapSurplus { get; set; } = decimal.Zero;
}
