using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;
using static FootballGm.Api.Data.Comparers;

namespace FootballGm.Api.Helpers
{
    public static class BudgetHelper
    {
        public static decimal[] CreatePaymentSchedule(List<Data.Models.Contract> contracts, bool includeSigningBonus = true)
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
                    if (week > contracts[contract].EndWeek || week < contracts[contract].StartWeek)
                        continue;

                    if (week == contracts[contract].StartWeek)
                        obligation -= contracts[contract].GiftedCapSpace;

                    var totalCompensation = includeSigningBonus
                        ? decimal.Add(contracts[contract].Salary, contracts[contract].SigningBonus)
                        : contracts[contract].Salary;

                    obligation += decimal.Divide(totalCompensation, (contracts[contract].EndWeek - contracts[contract].StartWeek + 1));     
                };

                if (decimal.Equals(obligation, decimal.Zero))
                    break;

                weekObligations[week] = Math.Round(obligation,2);
            }

            return weekObligations;
        }

        public static (bool TeamA, bool TeamB) ValidateProposedBudgets(
            List<Data.Models.Contract> tradesFromTeamA,
            List<Data.Models.Contract> tradesFromTeamB,
            Data.Models.Budget budgetA,
            Data.Models.Budget budgetB,
            decimal capCeiling)
        {
            (bool teamAValid, bool teamBValid) = (true, true);

            var paymentsFromA = CreatePaymentSchedule(tradesFromTeamA, false);
            var paymentsFromB = CreatePaymentSchedule(tradesFromTeamB, false);

            var tradeDiff = PaymentScheduleOperation(paymentsFromA, paymentsFromB, (A,B) => A - B );

            var postTradePaymentScheduleA = PaymentScheduleOperation(budgetA.PaymentSchedule, tradeDiff, (A, d) => A - d);
            var postTradePaymentScheduleB = PaymentScheduleOperation(budgetB.PaymentSchedule, tradeDiff, (B, d) => B + d);

            for (var week = WeekHelper.CurrentWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                if (postTradePaymentScheduleA[week] > capCeiling)
                    teamAValid = false;

                if (postTradePaymentScheduleB[week] > capCeiling)
                    teamAValid = false;
            }

            return (teamAValid, teamBValid);
        }

        public static decimal GetContractRating(Data.Models.Contract contract)
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

        private static decimal[] PaymentScheduleOperation(decimal[] a, decimal[] b, Func<decimal,decimal,decimal> operand)
        {
            var diff = new decimal[WeekHelper.NumberOfWeeksInSeason + 1];

            for (var week = 0; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                diff[week] = operand(a[week], b[week]);
            }

            return diff;
        }
    }
}
