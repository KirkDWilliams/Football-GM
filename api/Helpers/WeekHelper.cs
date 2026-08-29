namespace FootballGm.Api.Helpers;

public static class WeekHelper
{
    private static readonly List<DateOnly> NflWeeks =
    [
        new(2026, 09, 09), // Anything prior to this date should be considered a week 0.
        new(2026, 09, 14), // Otherwise, the index is the week in question.
        new(2026, 09, 21),
        new(2026, 09, 28),
        new(2026, 10, 05),
        new(2026, 10, 12),
        new(2026, 10, 19),
        new(2026, 10, 26),
        new(2026, 11, 02),
        new(2026, 11, 09),
        new(2026, 11, 16),
        new(2026, 11, 23),
        new(2026, 11, 30),
        new(2026, 12, 07),
        new(2026, 12, 14),
        new(2026, 12, 21),
        new(2026, 12, 28),
        new(2027, 01, 04),
        new(2027, 01, 10)
    ];

    public static int NumberOfWeeksInSeason => NflWeeks.Count - 1;

    public static int CurrentWeek
    {
        get
        {
            var today = DateOnly.FromDateTime(NowProvider());

            for (var week = 0; week < NumberOfWeeksInSeason; week++)
            {
                if (today < NflWeeks[week])
                    return week;
            }

            return -1;
        }
    }

    #region Testing Functions
    public static Func<DateTime> NowProvider { get; set; } = () => DateTime.Now;
    #endregion
}
