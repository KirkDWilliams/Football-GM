namespace FootballGm.Api.Data.Models;

public record Budget
{
    public int TeamId { get; set; }

    public required decimal[] PaymentSchedule { get; set; }

    public static Budget FromEntity(Entity.Contrived.Budget budget) => new()
    {
        TeamId = budget.TeamId,
        PaymentSchedule = budget.PaymentSchedule
    };
}
