namespace FootballGm.Api.Data.Entity.Ingested
{
    public class InjuryStatus
    {
        public short Season { get; set; } = 0;                           
        public short Week { get; set; } = 0;                             
        public string PlayerId { get; set; } = string.Empty;             
        public string OfficialReportStatus { get; set; } = string.Empty; 
        public string PracticePrimaryStatus { get; set; } = string.Empty;
        public string PracticeStatus { get; set; } = string.Empty;       
        public string LastUpdated { get; set; } = string.Empty;          
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
