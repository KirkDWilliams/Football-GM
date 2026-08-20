using FootballGm.Api.Data.Models;
using FootballGm.Api.Helpers;

namespace FootballGm.Api.Tests;

public class TeamBudgetTests(TeamBudget teamBudget) : IClassFixture<TeamBudget>
{
    private readonly TeamBudget _teamBudget = teamBudget;
    private readonly decimal roundingTolerance = .01M;

    [Fact]
    public void GivenEmptyContract_WhenCalculatingBudgetObligations_ReturnsEmpty()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 09);

            // Act
            var obligation = _teamBudget.GetBudgetObligations();

            // Assert
            Assert.Equal(Helpers.WeekHelper.NumberOfWeeksInSeason - 1, obligation.Length);
            Assert.True(obligation.All(obl => obl.Equals(decimal.Zero)));
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }

    [Fact]
    public void GivenSingleWeekContract_WhenCalculatingBudgetObligations_ReturnsExpected()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 09);

            _teamBudget.Contracts =
            [
                new()
                {
                    StartWeek = 1,
                    EndWeek = 1,
                    Salary = 1,
                    SigningBonus = 1
                }
            ];

            // Act
            var obligation = _teamBudget.GetBudgetObligations();

            // Assert
            Assert.Equal(0, obligation[0]);
            Assert.Equal(2, obligation[1]);
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }

    [Fact]
    public void GivenMultiWeekContract_WhenCalculatingBudgetObligations_ReturnsExpected()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 09);

            _teamBudget.Contracts =
            [
                new()
                {
                    StartWeek = 1,
                    EndWeek = 3,
                    Salary = 2,
                    SigningBonus = 3
                },
            ];

            // Act
            var obligation = _teamBudget.GetBudgetObligations();

            // Assert
            Assert.Equal(0, obligation[0]);
            Assert.True(3.666M - obligation[1] < roundingTolerance);
            Assert.True(0.666M - obligation[2] < roundingTolerance);
            Assert.True(0.666M - obligation[3] < roundingTolerance);
            Assert.Equal(0, obligation[4]);
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }

    [Fact]
    public void GivenManySingleWeekContracts_WhenCalculatingBudgetObligations_ReturnsExpected()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 09);

            _teamBudget.Contracts =
            [
                new()
                {
                    StartWeek = 1,
                    EndWeek = 1,
                    Salary = 1,
                    SigningBonus = 1
                },
                new()
                {
                    StartWeek = 1,
                    EndWeek = 1,
                    Salary = 1,
                    SigningBonus = 1
                },
                new()
                {
                    StartWeek = 1,
                    EndWeek = 1,
                    Salary = 1,
                    SigningBonus = 1
                }
            ];

            // Act
            var obligation = _teamBudget.GetBudgetObligations();

            // Assert
            Assert.Equal(0, obligation[0]);
            Assert.Equal(6, obligation[1]);
            Assert.Equal(0, obligation[2]);
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }
}
