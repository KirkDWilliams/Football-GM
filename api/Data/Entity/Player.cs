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
/*
 * R
    install.packages(c("DBI", "RSQLite", "nflreadr"))

    library(DBI)
    library(RSQLite)
    library(nflreadr)

    all_player_summary <- load_players()

    off_player_info <- all_player_summary[
        all_player_summary$position %in% c("QB", "RB", "WR", "TE", "K", "P"),
    ]

    player_info <- data.frame(
	    PlayerId = off_player_info$gsis_id,
	    Name = off_player_info$display_name,
	    PictureUrl = off_player_info$headshot,
	    Team = off_player_info$latest_team,
	    Position = off_player_info$position,
	    JerseyNumber = off_player_info$jersey_number,
	    DraftYear = off_player_info$rookie_season
    )
 */
