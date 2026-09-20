using FootballGm.Api.Domain.Helpers;

namespace FootballGm.Api.Tests;

[Collection("WeekClock")]
public class WeekHelperTests : IDisposable
{
    private readonly Func<DateTime> _original = WeekHelper.NowProvider;

    [Theory]
    [InlineData(2026, 9, 08, 0)]
    [InlineData(2026, 9, 09, 1)]
    [InlineData(2026, 9, 13, 1)]
    [InlineData(2026, 9, 14, 2)]
    [InlineData(2026, 11, 25, 12)]
    public void GivenValidDate_ShouldReturnExpectedWeek(int year, int month, int day, int expectedWeek) 
    {
        // Arrange
        WeekHelper.NowProvider = () => new DateTime(year, month, day);

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

    public void Dispose() => WeekHelper.NowProvider = _original;
}
