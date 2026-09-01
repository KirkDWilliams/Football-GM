using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Helpers
{
    public static class BudgetHelper
    {
        public static decimal[] CreatePaymentSchedule(List<Data.Models.Contract> contracts, ContractType type)
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

                    var weeklyPayment = decimal.Zero;

                    switch (type)
                    {
                        case ContractType.Standard:
                            weeklyPayment += contracts[contract].Salary + contracts[contract].SigningBonus;
                            break;

                        case ContractType.Received:
                            weeklyPayment += contracts[contract].Salary;
                            break;

                        case ContractType.Traded:
                        case ContractType.Dropped:
                            weeklyPayment += contracts[contract].SigningBonus;
                            break;

                        default: break;
                    }

                    obligation += weeklyPayment / (contracts[contract].EndWeek - contracts[contract].StartWeek + 1);     
                };

                if (decimal.Equals(obligation, decimal.Zero))
                    break;

                weekObligations[week] = Math.Round(obligation,2);
            }

            return weekObligations;
        }

        public static (decimal[] Salary, decimal[] Bonus) CreatePaymentSchedule(List<Data.Models.Contract> contracts)
        {
            var salaryObligations = new decimal[WeekHelper.NumberOfWeeksInSeason + 1];
            var bonusObligations = new decimal[WeekHelper.NumberOfWeeksInSeason + 1];

            var startingWeek = WeekHelper.CurrentWeek;

            if (startingWeek == 0)
                startingWeek++;

            for (var week = startingWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                var salaryObligation = decimal.Zero;
                var bonusObligation = decimal.Zero;

                for (var contract = 0; contract < contracts.Count; contract++)
                {
                    if (week > contracts[contract].EndWeek || week < contracts[contract].StartWeek)
                        continue;

                    if (week == contracts[contract].StartWeek)
                    {
                        salaryObligation -= contracts[contract].GiftedCapSpace / 2;
                        bonusObligation -= contracts[contract].GiftedCapSpace / 2; // TODO: NOT SURE WHICH TO GIVE CREDIT TO
                    }

                    var salaryPayment = contracts[contract].Salary;
                    var bonusPayment  = contracts[contract].SigningBonus;

                    salaryObligation += salaryPayment / (contracts[contract].EndWeek - contracts[contract].StartWeek + 1);
                    bonusObligation += bonusPayment / (contracts[contract].EndWeek - contracts[contract].StartWeek + 1);
                };

                if (salaryObligation == 0 && bonusObligation == 0)
                    break;

                salaryObligations[week] = Math.Round(salaryObligation, 2);
                bonusObligations[week] = Math.Round(bonusObligation, 2);
            }

            return (salaryObligations, bonusObligations);
        }

        public static (bool TeamA, bool TeamB) ValidateProposedBudgets(
            List<Data.Models.Contract> tradesFromTeamA,
            List<Data.Models.Contract> tradesFromTeamB,
            Data.Models.Budget budgetA,
            Data.Models.Budget budgetB,
            decimal capCeiling)
        {
            (bool teamAValid, bool teamBValid) = (true, true);

            (var salaryObligationToB, _) = CreatePaymentSchedule(tradesFromTeamA);
            (var salaryObligationToA, _) = CreatePaymentSchedule(tradesFromTeamB);

            // X budget: X current - X salary going away + Y salary coming in
            var newABudget = PaymentScheduleOperation(budgetA.PaymentSchedule, salaryObligationToB, salaryObligationToA, (a, b, c) => a - b + c);
            var newBBudget = PaymentScheduleOperation(budgetB.PaymentSchedule, salaryObligationToB, salaryObligationToA, (a, b, c) => a - b + c);

            for (var week = WeekHelper.CurrentWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                if (newABudget[week] > capCeiling)
                    teamAValid = false;

                if (newBBudget[week] > capCeiling)
                    teamAValid = false;
            }

            return (teamAValid, teamBValid);
        }

        public static decimal GetContractRating(Data.Models.Contract contract)
        {
            if (contract.StartWeek - 1 != WeekHelper.CurrentWeek)
                throw new InvalidOperationException("Contracts must be made one week prior to starting.");

            var paymentSchedule = CreatePaymentSchedule([contract], ContractType.Standard);

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

        private static decimal[] PaymentScheduleOperation(decimal[] a, decimal[] b, decimal[] c, Func<decimal,decimal,decimal,decimal> operand)
        {
            var diff = new decimal[WeekHelper.NumberOfWeeksInSeason + 1];

            for (var week = 0; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                diff[week] = operand(a[week], b[week], c[week]);
            }

            return diff;
        }
    }
}
