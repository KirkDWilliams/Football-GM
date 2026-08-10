namespace FootballGm.Api.Data.Entity
{
    public class Game // Schedules
    {
        public string GameId { get; set; } = string.Empty;   // game_id
        public short Season { get; set; }                    // season
        public short Week { get; set; }                      // week
        public DateOnly Date { get; set; } = new DateOnly(); // gameday
        public string HomeTeam { get; set; } = string.Empty; // home_team
        public string AwayTeam { get; set; } = string.Empty; // away_team
        public string HomeScore { get; set; } = "NA";        // home_score
        public string AwayScore { get; set; } = "NA";        // away_score
        public short? WindSpeed { get; set; } = 0;           // wind :)
        public short? Temperature { get; set; }              // temp :)
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
