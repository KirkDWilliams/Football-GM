using FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Data
{
    public class Comparers
    {
        public class ContractComparer : IEqualityComparer<Contract>
        {
            public bool Equals(Contract? x, Contract? y)
                => x is not null && y is not null && x.ContractId == y.ContractId;

            public int GetHashCode(Contract obj)
                => obj.ContractId.GetHashCode();
        }
    }
}
