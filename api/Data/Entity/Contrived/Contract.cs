using FootballGm.Api.Data.Entity.Associations;

namespace FootballGm.Api.Data.Entity.Contrived
{
    public class Contract
    {
        public int ContractId { get; set; }
        public int StartWeek { get; set; } = 0;
        public int EndWeek { get; set; } = 0;
        public int SigningBonus { get; set; } = 0;
        public int Salary { get; set; } = 0;

        public ICollection<TeamPlayers> TeamPlayers { get; set; } = [];

    }
}
