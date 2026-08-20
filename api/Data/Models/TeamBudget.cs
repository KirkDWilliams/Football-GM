using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Helpers;

namespace FootballGm.Api.Data.Models;

public class TeamBudget
{
    public List<Contract> Contracts { get; set; } = [];

    public decimal[] GetBudgetObligations()
    {
        var weekObligations = new decimal[WeekHelper.NumberOfWeeksInSeason - WeekHelper.CurrentWeek];

        for (var week = WeekHelper.CurrentWeek; week < WeekHelper.NumberOfWeeksInSeason; week++)
        {
            var obligation = decimal.Zero;

            foreach (var contract in Contracts)
            {
                if (week > contract.EndWeek)
                    break;

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
}
