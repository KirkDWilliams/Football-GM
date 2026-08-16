namespace FootballGm.Api.Helpers;

public class WeekHelper
{
    private static readonly List<DateOnly> nflWeeks = 
    [
        new DateOnly(2026, 09, 14),
        new DateOnly(2026, 09, 21),
        new DateOnly(2026, 09, 28),
        new DateOnly(2026, 10, 05),
        new DateOnly(2026, 10, 12),
        new DateOnly(2026, 10, 19),
        new DateOnly(2026, 10, 26),
        new DateOnly(2026, 11, 02),
        new DateOnly(2026, 11, 09),
        new DateOnly(2026, 11, 16),
        new DateOnly(2026, 11, 23),
        new DateOnly(2026, 11, 30),
        new DateOnly(2026, 12, 07),
        new DateOnly(2026, 12, 14),
        new DateOnly(2026, 12, 21),
        new DateOnly(2026, 12, 28),
        new DateOnly(2027, 01, 04),
        new DateOnly(2026, 01, 10),
    ];

    public int GetCurrentWeek()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        for (var ind = 0; ind < nflWeeks.Count; ind++)
        {
            if (today < nflWeeks[ind])
                return ind + 1;
        }

        return -1;
    }
}
