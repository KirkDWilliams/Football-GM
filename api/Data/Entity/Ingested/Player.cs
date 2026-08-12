using FootballGm.Api.Data.Entity.Associations;
using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Ingested
{
    public class Player
    {
        [Key]
        public string PlayerId { get; set; } = string.Empty;   
        public string Name { get; set; } = string.Empty;       
        public string PictureUrl {  get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;       
        public string Position { get; set; } = string.Empty;   
        public short JerseyNumber { get; set; }                
        public short DraftYear { get; set; }

        public ICollection<TeamPlayers> TeamPlayers { get; set; } = new List<TeamPlayers>();
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
