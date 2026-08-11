namespace FootballGm.Api.Data.Entity
{
    public class InjuryStatus // load_injuries()
    {
        public short Season { get; set; } = 0;                            // season
        public short Week { get; set; } = 0;                              // week
        public string PlayerId { get; set; } = string.Empty;              // gsis_id
        public string OfficialReportStatus { get; set; } = string.Empty;  // report_primary_injury
        public string PracticePrimaryStatus { get; set; } = string.Empty; // practice_primary_injury
        public string PracticeStatus { get; set; } = string.Empty;        // practice_status
        public string LastUpdated { get; set; } = string.Empty;           // date_modified
    }
}

/*   
    R
 
    all_injuries <- load_injuries(2026)

    off_injuries <- all_injuries[
        all_injuries$position %in% c("QB", "RB", "WR", "TE", "K", "P"),
    ]

    injuries <- data.frame(
	    GameId = off_injuries$season,
	    Week = off_injuries$week,
	    PlayerId = off_injuries$gsis_id,
	    OfficialReportStatus = off_injuries$report_primary_injury,
	    PracticePrimaryStatus = off_injuries$practice_primary_injury,
	    PracticeStatus = off_injuries$practice_status
    )
 */
