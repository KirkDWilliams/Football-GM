namespace FootballGm.Api.Data.Models;

public record Contract
{
    public Contract(Entity.Contrived.Contract contract)
    {
        ContractId = contract.ContractId;
        StartWeek = contract.StartWeek;
        EndWeek = contract.EndWeek;
        SigningBonus = contract.SigningBonus;
        Salary = contract.Salary;
        GiftedCapSpace = contract.GiftedCapSpace;
    }

    public Contract() { }

    public int ContractId { get; set; }
    public int StartWeek { get; set; }
    public int EndWeek { get; set; } // a contract is good thru this week (e.g. for EndWeek of 5, the contract is finished by start of week 6)
    public float SigningBonus { get; set; } = 0f;
    public float Salary { get; set; } = 0f;
    public float GiftedCapSpace { get; set; } = 0f;

    public static Contract FromEntity(Entity.Contrived.Contract contract) => new(contract);
}
