using FootballGm.Api.Helpers;

namespace FootballGm.Api.Tests;

public class WeekHelperTests
{
    [Theory]
    [InlineData(9, 08, 0)]
    [InlineData(9, 09, 1)]
    [InlineData(9, 13, 1)]
    [InlineData(9, 14, 2)]
    public void GivenValidDate_ShouldReturnExpectedWeek(int month, int day, int expectedWeek) 
    {
        // Arrange
        WeekHelper.NowProvider = () => new DateTime(2026, month, day);

        // Act
        var week = WeekHelper.CurrentWeek;

        // Assert
        Assert.Equal(expectedWeek, week);
    }

    [Fact]
    public void GivenDatePostRegularSeason_ShouldReturnInvalidNumeric()
    {
        // Arrange
        WeekHelper.NowProvider = () => new DateTime(2027, 1, 11);

        // Act
        var week = WeekHelper.CurrentWeek;

        // Assert
        Assert.Equal(-1, week);
    }
}
