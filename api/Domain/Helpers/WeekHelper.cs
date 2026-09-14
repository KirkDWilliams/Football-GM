namespace FootballGm.Api.Domain.Helpers;

public static class WeekHelper
{
    private static readonly List<DateOnly> NflWeeks =
    [                      // 0 
        new(2026, 09, 09), // 1.
        new(2026, 09, 14), // 2, Otherwise, the index is the week in question.
        new(2026, 09, 21), // 3 
        new(2026, 09, 28), // 4
        new(2026, 10, 05), // 5
        new(2026, 10, 12), // 6
        new(2026, 10, 19), // 7
        new(2026, 10, 26), // 8
        new(2026, 11, 02), // 9
        new(2026, 11, 09), // 10
        new(2026, 11, 16), // 11
        new(2026, 11, 23), // 12 (this week begins on Wednesday, so everything needs to be slid backwards.)
        new(2026, 11, 30), // 13
        new(2026, 12, 07), // 14
        new(2026, 12, 14), // 15
        new(2026, 12, 21), // 16
        new(2026, 12, 28), // 17
        new(2027, 01, 04), // 18
        new(2027, 01, 10)  // fin.
    ];

    public static int NumberOfWeeksInSeason => NflWeeks.Count - 1;

    public static short CurrentSeason => (short)NflWeeks[0].Year;

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
