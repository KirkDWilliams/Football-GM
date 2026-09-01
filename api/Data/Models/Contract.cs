namespace FootballGm.Api.Data.Models
{
    public record Contract
    {
        public int ContractId { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; } // a contract is good thru this week (e.g. for EndWeek of 5, the contract is finished by start of week 6)
        public decimal SigningBonus { get; set; } = decimal.Zero;
        public decimal Salary { get; set; } = decimal.Zero;
        public decimal GiftedCapSpace { get; set; } = decimal.Zero;

        public static Contract FromEntity(Entity.Contrived.Contract contract) => new()
        {
            ContractId = contract.ContractId,
            StartWeek = contract.StartWeek,
            EndWeek = contract.EndWeek,
            SigningBonus = contract.SigningBonus,
            Salary = contract.Salary,
            GiftedCapSpace = contract.GiftedCapSpace
        };
    }
}
