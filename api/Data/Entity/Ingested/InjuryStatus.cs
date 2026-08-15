namespace FootballGm.Api.Data.Entity.Ingested;

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
