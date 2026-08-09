namespace FootballGm.Api.Data.Entity
{
    public class Player // load_players()
    {
        public string PlayerId { get; set; } = string.Empty;    // gsis_id
        public string Name { get; set; } = string.Empty;        // display_name
        public string PictureUrl {  get; set; } = string.Empty; // headshot
        public string Team { get; set; } = string.Empty;        // latest_team
        public string Position { get; set; } = string.Empty;    // position
        public short JerseyNumber { get; set; }                 // jersey_number
        public short DraftYear { get; set; }                    // rookie_season
    }
}
