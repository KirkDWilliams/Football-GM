using FootballGm.Api.Data.Models;
using FootballGm.Api.Helpers;

namespace FootballGm.Api.Tests;

public class BudgetHelperTests()
{
    private readonly decimal roundingTolerance = .01M;

    #region Payment Schedule

    [Fact]
    public void GivenEmptyContract_WhenCalculatingBudgetObligations_ReturnsEmpty()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 09);

            // Act
            var obligation = BudgetHelper.CreatePaymentSchedule([], Data.Enums.ContractType.Standard);

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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts, Data.Enums.ContractType.Standard);

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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts, Data.Enums.ContractType.Standard);

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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts, Data.Enums.ContractType.Standard);

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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts, Data.Enums.ContractType.Standard);

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
            var obligation = BudgetHelper.CreatePaymentSchedule(contracts, Data.Enums.ContractType.Standard);

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

    #endregion

    #region Contract Ratings

    [Fact]
    public void GivenContractOptions_CeterisPeribus_ShorterContractsArePreferred()
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

            Contract fiveWeekContract = new()
            {
                StartWeek = 1,
                EndWeek = 5,
                Salary = 50,
                SigningBonus = 4
            };

            Contract fourWeekContract = new()
            {
                StartWeek = 1,
                EndWeek = 4,
                Salary = 50,
                SigningBonus = 4
            };

            // Act
            var sixWeekRating = BudgetHelper.GetContractRating(sixWeekContract);
            var fiveWeekRating = BudgetHelper.GetContractRating(fiveWeekContract);
            var fourWeekRating = BudgetHelper.GetContractRating(fourWeekContract);

            // Assert
            Assert.True(fiveWeekRating > sixWeekRating);
            Assert.True(fourWeekRating > fiveWeekRating);
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }

    [Fact]
    public void GivenContractOptions_CeterisPeribus_GreaterSalariesArePreferred()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 08);

            Contract highestSalary = new()
            {
                StartWeek = 1,
                EndWeek = 5,
                Salary = 51,
                SigningBonus = 4
            };

            Contract middleSalary = new()
            {
                StartWeek = 1,
                EndWeek = 5,
                Salary = 50,
                SigningBonus = 4
            };

            Contract lowestSalary = new()
            {
                StartWeek = 1,
                EndWeek = 5,
                Salary = 49,
                SigningBonus = 4
            };

            // Act
            var highestRating = BudgetHelper.GetContractRating(highestSalary);
            var middleRating = BudgetHelper.GetContractRating(middleSalary);
            var lowestRating = BudgetHelper.GetContractRating(lowestSalary);

            // Assert
            Assert.True(middleRating < highestRating);
            Assert.True(lowestRating < middleRating);
        }
        finally
        {
            WeekHelper.NowProvider = original;
        }
    }

    [Fact]
    public void GivenContractOptions_CeterisPeribus_GreaterSigningBonusesArePreferred()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 09, 08);

            Contract highestBonus = new()
            {
                StartWeek = 1,
                EndWeek = 5,
                Salary = 50,
                SigningBonus = 5
            };

            Contract middleBonus = new()
            {
                StartWeek = 1,
                EndWeek = 5,
                Salary = 50,
                SigningBonus = 4
            };

            Contract lowestBonus = new()
            {
                StartWeek = 1,
                EndWeek = 5,
                Salary = 50,
                SigningBonus = 3
            };

            // Act
            var highestRating = BudgetHelper.GetContractRating(highestBonus);
            var middleRating = BudgetHelper.GetContractRating(middleBonus);
            var lowestRating = BudgetHelper.GetContractRating(lowestBonus);

            // Assert
            Assert.True(middleRating < highestRating);
            Assert.True(lowestRating < middleRating);
        }
        finally { WeekHelper.NowProvider = original; }
    }

    #endregion

    #region ValidateBudgets

    [Fact]
    public void GivenTwoTeamsWithAValidTrade_WhenValidatingTheTradeProposalForBudgetExcess_ReturnsTrueForEach()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 11, 15); //10

            List<Contract> proposedTradesFromTeamA =
            [
                new() { ContractId = 100, StartWeek = 9, EndWeek = 15, Salary = 45, SigningBonus = 10 },
                new() { ContractId = 101, StartWeek = 8, EndWeek = 15, Salary = 45, SigningBonus = 10 }
            ];

            List<Contract> proposedTradesFromTeamB =
            [
                new() { ContractId = 200, StartWeek = 7,  EndWeek = 14, Salary = 60, SigningBonus = 4 },
                new() { ContractId = 201, StartWeek = 10, GiftedCapSpace = 15 }
            ];

            var teamBudgetA = new Data.Models.Budget
            {
                TeamId = 1,
                PaymentSchedule = BudgetHelper.CreatePaymentSchedule(
                [
                    new() { ContractId = 100, StartWeek = 9, EndWeek = 15, Salary = 45, SigningBonus = 10 },
                    new() { ContractId = 101, StartWeek = 8, EndWeek = 15, Salary = 45, SigningBonus = 10 },
                    new() { ContractId = 102, StartWeek = 5, EndWeek = 12, Salary = 65, SigningBonus = 12 }
                ], Data.Enums.ContractType.Standard)
            };

            var teamBudgetB = new Data.Models.Budget
            {
                TeamId = 2,
                PaymentSchedule = BudgetHelper.CreatePaymentSchedule(
                [
                    new() { ContractId = 200, StartWeek = 7,  EndWeek = 14, Salary = 60, SigningBonus = 4 },
                    new() { ContractId = 200, StartWeek = 8,  EndWeek = 15, Salary = 60, SigningBonus = 14 },
                    new() { ContractId = 200, StartWeek = 9,  EndWeek = 16, Salary = 60, SigningBonus = 24 },
                    new() { ContractId = 201, StartWeek = 10, GiftedCapSpace = 15 }
                ], Data.Enums.ContractType.Standard)
            };

            // Act
            var (teamABudgetValidity, teamBBudgetValidity) = BudgetHelper.ValidateProposedBudgets(proposedTradesFromTeamA, proposedTradesFromTeamB, teamBudgetA, teamBudgetB, 50);

            // Assert
            Assert.True(teamABudgetValidity);
            Assert.True(teamBBudgetValidity);
        }
        finally { WeekHelper.NowProvider = original; }
    }

    [Fact]
    public void GivenTwoTeamsWithAnInvalidTrade_WhenValidatingTheTradeProposalForBudgetExcess_ReturnsFalseAndTrue()
    {
        // Arrange
        var original = WeekHelper.NowProvider;
        try
        {
            WeekHelper.NowProvider = () => new DateTime(2026, 11, 15); //10

            List<Contract> proposedTradesFromTeamA =
            [
                new() { ContractId = 100, StartWeek = 9, EndWeek = 15, Salary = 45, SigningBonus = 10 },
                new() { ContractId = 101, StartWeek = 8, EndWeek = 15, Salary = 45, SigningBonus = 10 }
            ];

            List<Contract> proposedTradesFromTeamB =
            [
                new() { ContractId = 200, StartWeek = 7,  EndWeek = 14, Salary = 60, SigningBonus = 4 },
                new() { ContractId = 201, StartWeek = 10, GiftedCapSpace = 15 }
            ];

            var teamBudgetA = new Data.Models.Budget
            {
                TeamId = 1,
                PaymentSchedule = BudgetHelper.CreatePaymentSchedule(
                [
                    new() { ContractId = 100, StartWeek = 9, EndWeek = 15, Salary = 45, SigningBonus = 10 },
                    new() { ContractId = 101, StartWeek = 8, EndWeek = 15, Salary = 45, SigningBonus = 10 },
                    new() { ContractId = 102, StartWeek = 5, EndWeek = 12, Salary = 65, SigningBonus = 12 },
                ], Data.Enums.ContractType.Standard)
            };

            var teamBudgetB = new Data.Models.Budget
            {
                TeamId = 2,
                PaymentSchedule = BudgetHelper.CreatePaymentSchedule(
                [
                    new() { ContractId = 200, StartWeek = 7,  EndWeek = 14, Salary = 60, SigningBonus = 4 },
                    new() { ContractId = 200, StartWeek = 8,  EndWeek = 15, Salary = 60, SigningBonus = 14 },
                    new() { ContractId = 200, StartWeek = 9,  EndWeek = 16, Salary = 60, SigningBonus = 24 },
                    new() { ContractId = 201, StartWeek = 10, GiftedCapSpace = 15 }
                ], Data.Enums.ContractType.Standard)
            };

            // Act
            var (teamABudgetValidity, teamBBudgetValidity) = BudgetHelper.ValidateProposedBudgets(proposedTradesFromTeamA, proposedTradesFromTeamB, teamBudgetA, teamBudgetB, 15);

            // Assert
            Assert.False(teamABudgetValidity);
            Assert.True(teamBBudgetValidity);
        }
        finally { WeekHelper.NowProvider = original; }
    }

    #endregion
}
