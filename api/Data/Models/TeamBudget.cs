using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Data.Models;

public class TeamBudget
{
    public int TeamId { get; set; }

    public List<Contract> Contracts { get; set; } = [];
}
