using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Helpers
{
    public static class BudgetHelper
    {
        public static decimal[] CreatePaymentSchedule(List<Contract> contracts)
        {
            var weekObligations = new decimal[WeekHelper.NumberOfWeeksInSeason+1];
            var startingWeek = WeekHelper.CurrentWeek;

            if (startingWeek == 0)
                startingWeek++;

            for (var week = startingWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                var obligation = decimal.Zero;

                for (var contract = 0; contract < contracts.Count; contract++)
                {
                    if (week == contracts[contract].StartWeek && contracts[contract].GiftedCapSpace > decimal.Zero)
                        obligation -= contracts[contract].GiftedCapSpace;

                    if (week > contracts[contract].EndWeek)
                        continue;

                    if (week == contracts[contract].StartWeek)
                        obligation += contracts[contract].SigningBonus;

                    if (week >= contracts[contract].StartWeek)
                        obligation += decimal.Divide(contracts[contract].Salary, (contracts[contract].EndWeek - contracts[contract].StartWeek + 1));
                };

                if (decimal.Equals(obligation, decimal.Zero))
                    break;

                weekObligations[week] = obligation;
            }

            return weekObligations;
        }

        public static (bool TeamA, bool TeamB) ValidateBudgets(
            List<Contract> a_ContractsTrading,
            List<Contract> b_ContractsTrading,
            TeamBudget budgetA,
            TeamBudget budgetB,
            decimal capCeiling)
        {
            (bool teamAIsValid, bool teamBIsValid) = (true, true);

            var a_TradeContractIds = a_ContractsTrading.Select(trade => trade.ContractId);
            var b_TradeContractIds = b_ContractsTrading.Select(trade => trade.ContractId);

            budgetA.Contracts.RemoveAll(c => a_TradeContractIds.Contains(c.ContractId));
            budgetB.Contracts.RemoveAll(c => b_TradeContractIds.Contains(c.ContractId));

            var a_ProposedContracts = budgetA.Contracts.Concat(b_ContractsTrading).ToList();
            var b_ProposedContracts = budgetB.Contracts.Concat(a_ContractsTrading).ToList();

            var a_ProposedPayments = CreatePaymentSchedule(a_ProposedContracts);
            var b_ProposedPayments = CreatePaymentSchedule(b_ProposedContracts);

            for (var week = WeekHelper.CurrentWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                if (a_ProposedPayments[week] > capCeiling)
                    teamAIsValid = false;

                if (b_ProposedPayments[week] > capCeiling)
                    teamAIsValid = false;
            }

            return (teamAIsValid, teamBIsValid);
        }

        public static decimal GetContractRating(Contract contract)
        {
            if (contract.StartWeek - 1 != WeekHelper.CurrentWeek)
                throw new InvalidOperationException("Contracts must be made one week prior to starting.");

            var paymentSchedule = CreatePaymentSchedule([contract]);

            var rating = decimal.Zero;
            var week = contract.StartWeek;
            var discount = 0.0625D;

            do
            {
                rating += Decimal.Add(1, Decimal.Multiply(paymentSchedule[week], (decimal)Math.Pow(1 - discount, week++ - (contract.StartWeek - 1))));
            }
            while (paymentSchedule[week] > 0);


            return rating;
        }
    }
}
