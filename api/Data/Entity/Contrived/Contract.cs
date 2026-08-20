using FootballGm.Api.Data.Entity.Associations;

namespace FootballGm.Api.Data.Entity.Contrived;

public class Contract
{
    public int ContractId { get; set; }
    public int StartWeek { get; set; } = 0;
    public int EndWeek { get; set; } = 0; // a contract is good thru this week (e.g. for EndWeek of 5, the contract is finished by start of week 6) 
    public decimal SigningBonus { get; set; } = decimal.Zero; // this is paid at the beginning of the contract
    public decimal Salary { get; set; } = decimal.Zero; // this is spread about the duration of the season

    public ICollection<TeamPlayers> TeamPlayers { get; set; } = [];
}
