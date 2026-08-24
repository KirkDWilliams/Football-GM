using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;
using static FootballGm.Api.Data.Comparers;

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

                weekObligations[week] = Math.Round(obligation,2);
            }

            return weekObligations;
        }

        public static (bool TeamA, bool TeamB) ValidateProposedBudgets(
            List<Contract> tradesFromTeamA,
            List<Contract> tradesFromTeamB,
            TeamBudget budgetA,
            TeamBudget budgetB,
            decimal capCeiling)
        {
            (bool teamAValid, bool teamBValid) = (true, true);

            var remainingAContracts = budgetA.Contracts.Except(tradesFromTeamA, new ContractComparer());
            var remainingBContracts = budgetB.Contracts.Except(tradesFromTeamB, new ContractComparer());

            var proposedTeamAContracts = remainingAContracts.Concat(tradesFromTeamB).ToList();
            var proposedTeamBContracts = remainingBContracts.Concat(tradesFromTeamA).ToList();

            var teamAPaymentSchedule = CreatePaymentSchedule(proposedTeamAContracts);
            var teamBPaymentSchedule = CreatePaymentSchedule(proposedTeamBContracts);

            for (var week = WeekHelper.CurrentWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                if (teamAPaymentSchedule[week] > capCeiling)
                    teamAValid = false;

                if (teamBPaymentSchedule[week] > capCeiling)
                    teamAValid = false;
            }

            return (teamAValid, teamBValid);
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
                rating += Math.Round(Decimal.Add(1, Decimal.Multiply(paymentSchedule[week], (decimal)Math.Pow(1 - discount, week++ - (contract.StartWeek - 1)))),2);
            }
            while (paymentSchedule[week] > 0);

            return rating;
        }
    }
}
