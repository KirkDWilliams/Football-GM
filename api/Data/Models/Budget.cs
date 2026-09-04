namespace FootballGm.Api.Data.Models;

public record Budget
{
    public Budget(Entity.Contrived.Budget budget)
    {
        TeamId = budget.TeamId;
        PaymentSchedule = budget.PaymentSchedule;
    }

    public Budget() {}

    public int TeamId { get; set; }

    public decimal[] PaymentSchedule { get; set; } = [];

    public static Budget FromEntity(Entity.Contrived.Budget budget) => new (budget);
}
