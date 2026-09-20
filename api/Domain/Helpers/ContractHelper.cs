using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Domain.Helpers
{
    public static class ContractHelper
    {
        public static float[] CreatePaymentSchedule(List<Data.Models.Contract> contracts, ContractStatus type)
        {
            var weekObligations = new float[WeekHelper.NumberOfWeeksInSeason + 1];
            var startingWeek = WeekHelper.CurrentWeek;

            if (startingWeek == 0)
                startingWeek++;

            for (var week = startingWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                var obligation = 0f;

                for (var contract = 0; contract < contracts.Count; contract++)
                {
                    if (week > contracts[contract].EndWeek || week < contracts[contract].StartWeek)
                        continue;

                    if (week == contracts[contract].StartWeek)
                        obligation -= contracts[contract].GiftedCapSpace;

                    var weeklyPayment = 0f;
                    var duration = contracts[contract].EndWeek - contracts[contract].StartWeek + 1;

                    switch (type)
                    {
                        case ContractStatus.Standard:
                            weeklyPayment += contracts[contract].Salary + contracts[contract].SigningBonus;
                            break;

                        case ContractStatus.Received:
                            weeklyPayment += contracts[contract].Salary;
                            break;

                        case ContractStatus.Traded:
                        case ContractStatus.Dropped:
                            weeklyPayment += contracts[contract].SigningBonus;
                            break;

                        default: break;
                    }

                    obligation += weeklyPayment / duration;
                };

                if (obligation == 0f)
                    break;

                weekObligations[week] = (float)Math.Round(obligation,2);
            }

            return weekObligations;
        }

        public static (float[] Salary, float[] Bonus) CreatePaymentSchedule(List<Data.Models.Contract> contracts)
        {
            var salaryObligations = new float[WeekHelper.NumberOfWeeksInSeason + 1];
            var bonusObligations = new float[WeekHelper.NumberOfWeeksInSeason + 1];

            var startingWeek = WeekHelper.CurrentWeek;

            if (startingWeek == 0)
                startingWeek++;

            for (var week = startingWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                var salaryObligation = 0f;
                var bonusObligation = 0f;

                for (var contract = 0; contract < contracts.Count; contract++)
                {
                    if (week > contracts[contract].EndWeek || week < contracts[contract].StartWeek)
                        continue;

                    if (week == contracts[contract].StartWeek)
                    {
                        salaryObligation -= contracts[contract].GiftedCapSpace;
                    }

                    var salaryPayment = contracts[contract].Salary;
                    var bonusPayment  = contracts[contract].SigningBonus;

                    salaryObligation += salaryPayment / (contracts[contract].EndWeek - contracts[contract].StartWeek + 1);
                    bonusObligation += bonusPayment / (contracts[contract].EndWeek - contracts[contract].StartWeek + 1);
                };

                if (salaryObligation == 0 && bonusObligation == 0)
                    break;

                salaryObligations[week] = (float)Math.Round(salaryObligation, 2);
                bonusObligations[week] = (float)Math.Round(bonusObligation, 2);
            }

            return (salaryObligations, bonusObligations);
        }

        public static (bool TeamA, bool TeamB) ValidateProposedBudgets(
            List<Data.Models.Contract> tradesFromTeamA,
            List<Data.Models.Contract> tradesFromTeamB,
            Data.Models.Budget budgetA,
            Data.Models.Budget budgetB,
            float capCeiling)
        {
            (bool teamAValid, bool teamBValid) = (true, true);

            (var salaryObligationToB, _) = CreatePaymentSchedule(tradesFromTeamA);
            (var salaryObligationToA, _) = CreatePaymentSchedule(tradesFromTeamB);

            // X budget: X current - X salary going away + Y salary coming in
            var newABudget = PaymentScheduleOperation(budgetA.PaymentSchedule, salaryObligationToB, salaryObligationToA, (x, y, z) => x - y + z);
            var newBBudget = PaymentScheduleOperation(budgetB.PaymentSchedule, salaryObligationToA, salaryObligationToB, (x, y, z) => x - y + z);

            for (var week = WeekHelper.CurrentWeek; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                if (newABudget[week] > capCeiling)
                    teamAValid = false;

                if (newBBudget[week] > capCeiling)
                    teamBValid = false;
            }

            return (teamAValid, teamBValid);
        }

        public static float GetContractRating(Data.Models.Contract contract)
        {
            if (contract.StartWeek - 1 != WeekHelper.CurrentWeek)
                throw new InvalidOperationException("Contracts must be made one week prior to starting.");

            var paymentSchedule = CreatePaymentSchedule([contract], ContractStatus.Standard);

            var rating = 0f;
            var week = contract.StartWeek;
            var discount = 0.1865;

            do
            {
                rating += (float)Math.Round(
                            1 + paymentSchedule[week] * Math.Pow(
                                1 - discount, week++ - contract.StartWeek - 1)
                            , 2);
            }
            while (paymentSchedule[week] > 0);

            return rating;
        }

        private static float[] PaymentScheduleOperation(float[] a, float[] b, float[] c, Func<float,float,float,float> operand)
        {
            var diff = new float[WeekHelper.NumberOfWeeksInSeason + 1];

            for (var week = 0; week <= WeekHelper.NumberOfWeeksInSeason; week++)
            {
                diff[week] = operand(a[week], b[week], c[week]);
            }

            return diff;
        }
    }
}
