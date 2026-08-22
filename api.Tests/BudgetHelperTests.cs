using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Helpers;

namespace FootballGm.Api.Tests;

public class BudgetHelperTests()
{
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
            var obligation = BudgetHelper.CreatePaymentSchedule([]);

            // Assert
            Assert.Equal(WeekHelper.NumberOfWeeksInSeason+1, obligation.Length);
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

            List<Contract> contracts = 
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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts);

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

            List<Contract> contracts =
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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts);

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

            List<Contract> contracts =
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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts);

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

    [Fact]
    public void GivenManyMultiWeekContracts_AndStartingPastGivenContracts_WhenCalculatingBudgetObligations_ReturnsNoObligations()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 10, 26);

            List<Contract> contracts =
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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts);

            // Assert
            Assert.Equal(0, obligation[0]);
            Assert.Equal(19, obligation.Length);
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }

    [Fact]
    public void GivenManyMultiWeekContracts_AndStartingIntoGivenContracts_WhenCalculatingBudgetObligations_ReturnsNoObligations()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 10, 26);

            List<Contract> contracts =
            [
                new()
                {
                    StartWeek = 1,
                    EndWeek = 17,
                    Salary = 17,
                    SigningBonus = 13
                },
                new()
                {
                    StartWeek = 4,
                    EndWeek = 16,
                    Salary = 13,
                    SigningBonus = 10
                },
                new()
                {
                    StartWeek = 17,
                    EndWeek = 18,
                    Salary = 1,
                    SigningBonus = 1
                }
            ];

            // Act
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts);

            // Assert
            Assert.Equal(0, obligation[7]);
            Assert.Equal(2, obligation[8]);
            Assert.Equal(2, obligation[16]);
            Assert.Equal(2.5M, obligation[17]);
            Assert.Equal(19, obligation.Length);
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }

    [Fact]
    public void GivenContractAlreadyBegun_WhenCalculatingContractRating_ThrowsError()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 09);

            Contract contract = new()
            {
                StartWeek = 1,
                EndWeek = 6,
                Salary = 50,
                SigningBonus = 4
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>( () => BudgetHelper.GetContractRating(contract));
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }

    [Fact]
    public void GivenContractOptionsBetween_CeterisPeribus_ShorterContractsArePreferred()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 08);

            Contract sixWeekContract = new()
            {
                StartWeek = 1,
                EndWeek = 6,
                Salary = 50,
                SigningBonus = 4
            };

            // Act
            var sixWeekRating = BudgetHelper.GetContractRating(sixWeekContract);

            // Assert
            Assert.True(sixWeekRating < 50);
            Assert.True(sixWeekRating > 49);

            //Arrange 2
            Contract fiveWeekContract = new()
            {
                StartWeek = 1,
                EndWeek = 5,
                Salary = 50,
                SigningBonus = 4
            };

            // Act
            var fiveWeekRating = BudgetHelper.GetContractRating(fiveWeekContract);

            // Assert
            Assert.True(fiveWeekRating > sixWeekRating);

            //Arrange 3
            Contract fourWeekContract = new()
            {
                StartWeek = 1,
                EndWeek = 4,
                Salary = 50,
                SigningBonus = 4
            };

            // Act
            var fourWeekRating = BudgetHelper.GetContractRating(fourWeekContract);

            // Assert
            Assert.True(fourWeekRating > fiveWeekRating);
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }
}
