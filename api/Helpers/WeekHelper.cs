namespace FootballGm.Api.Helpers;

public static class WeekHelper
{
    private static readonly List<DateOnly> nflWeeks = 
    [
        new DateOnly(2026, 09, 09), // Anything prior to this date should be considered a week 0.
        new DateOnly(2026, 09, 14), // Otherwise, the index is the week in question.
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
        new DateOnly(2027, 01, 10),
    ];

    public static int GetCurrentWeek(DateOnly? fakeDate = null)
    {
        var today = fakeDate == null
            ? DateOnly.FromDateTime(DateTime.Now)
            : fakeDate;

        for (var week = 0; week < nflWeeks.Count; week++)
        {
            if (today < nflWeeks[week])
                return week;
        }

        return -1; // Not sure how you got here!
    }
}
