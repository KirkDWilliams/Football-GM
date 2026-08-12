using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Entity.Ingested
{
    public class Game
    {
        [Key]
        public string GameId { get; set; } = string.Empty;   
        public short Season { get; set; }                    
        public short Week { get; set; }                      
        public DateOnly Date { get; set; } = new DateOnly(); 
        public string HomeTeam { get; set; } = string.Empty; 
        public string AwayTeam { get; set; } = string.Empty; 
        public string HomeScore { get; set; } = "NA";        
        public string AwayScore { get; set; } = "NA";        
        public short? WindSpeed { get; set; } = 0;           
        public short? Temperature { get; set; }              
    }
}

/*
 * R
 * 
    all_games <-load_schedules(2026)

    games <- data.frame(
	    GameId = all_games$game_id,
	    Season = all_games$season,
	    Week = all_games$week,
	    Date = all_games$gameday,
	    HomeTeam = all_games$home_team,
	    AwayTeam = all_games$away_team,
	    HomeScore = all_games$home_score,
	    AwayScore = all_games$away_score,
	    WindSpeed = all_games$wind,
	    Temperature = all_games$temp
    )
 */
