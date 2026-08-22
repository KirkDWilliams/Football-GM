using FootballGm.Api.Data.Entity.Contrived;

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

                foreach (var contract in contracts)
                {
                    if (week > contract.EndWeek)
                        continue;

                    if (week == contract.StartWeek)
                        obligation += contract.SigningBonus;

                    if (week >= contract.StartWeek)
                        obligation += decimal.Divide(contract.Salary, (contract.EndWeek - contract.StartWeek + 1));
                };

                if (decimal.Equals(obligation, decimal.Zero))
                    break;

                weekObligations[week] = obligation;
            }

            return weekObligations;
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
                rating += 1 + Decimal.Multiply(paymentSchedule[week], (decimal)Math.Pow(1 - discount, week++ - (contract.StartWeek - 1)));
            }
            while (paymentSchedule[week] > 0);


            return rating;
        }
    }
}
