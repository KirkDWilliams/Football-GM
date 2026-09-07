using FootballGm.Api.Data.Entity.Associations;

namespace FootballGm.Api.Data.Entity.Contrived;

public class Contract
{
    public int ContractId { get; set; }
    public int StartWeek { get; set; }
    public int EndWeek { get; set; } // a contract is good thru this week (e.g. for EndWeek of 5, the contract is finished by start of week 6)
    public float SigningBonus { get; set; } = 0f;
    public float Salary { get; set; } = 0f;
    public float GiftedCapSpace { get; set;  } = 0f;

    // Navigation Properities
    public ICollection<TeamPlayers> TeamPlayers { get; set; } = [];
}
