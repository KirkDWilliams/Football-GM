using FootballGm.Api.Helpers;

namespace FootballGm.Api.Tests;

public class WeekHelperTests
{
    public WeekHelperTests(){}

    [Theory]
    [InlineData(9, 08, 0)]
    [InlineData(9, 09, 1)]
    [InlineData(9, 13, 1)]
    [InlineData(9, 14, 2)]
    public void GivenInvalidDate_ShouldReturnInvalidNumeric(int month, int day, int expectedWeek) 
    {
        // Arrange
        var dateOnly = new DateOnly(2026, month, day);

        // Act
        var week = WeekHelper.GetCurrentWeek(dateOnly);

        // Assert
        Assert.Equal(expectedWeek, week);
    }
}
