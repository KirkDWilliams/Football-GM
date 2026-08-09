namespace FootballGm.Api.Data.Entity
{
    public class InjuryStatus // load_injuries()
    {
        public short Season { get; set; } = 0;                            // Season
        public short Week { get; set; } = 0;                              // week
        public string PlayerId { get; set; } = string.Empty;              // gsis_id
        public string OfficialReportStatus { get; set; } = string.Empty;  // report_primary_injury
        public string PracticePrimaryStatus { get; set; } = string.Empty; // practice_primary_injury
        public string PracticeStatus { get; set; } = string.Empty;        // practice_status
        public string LastUpdated { get; set; } = string.Empty;           // date_modified
    }
}
