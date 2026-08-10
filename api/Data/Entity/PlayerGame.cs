namespace FootballGm.Api.Data.Entity
{
    public class PlayerGame
    {
        public string PlayerId { get; set; } = string.Empty; // player_id
        public string GameId { get; set; } = string.Empty;   // game_id

        public short PassAttempts { get; set; } = 0;        // attempts
        public short PassCompletions { get; set; } = 0;     // completions
        public short PassingYards { get; set; } = 0;        // passing_yards
        public short PassingTouchdowns { get; set; } = 0;   // passing_tds

        public short RushAttempts { get; set; } = 0;        // carries
        public short RushingYards { get; set; } = 0;        // rushing_yards
        public short RushingFirstDowns { get; set; } = 0;   // rushing_first_downs
        public short RushingTouchdowns { get; set; } = 0;   // rushing_tds

        public short Receptions { get; set; } = 0;          // receptions
        public short ReceivingYards { get; set; } = 0;      // receiving_yards
        public short ReceivingTouchdowns { get; set; } = 0; // receiving_tds


        public short Interceptions { get; set; } = 0;       // passing_interceptions
        public short Fumbles { get; set; } = 0;             // fumbles_lost_total
        public short Sacks { get; set; } = 0;               // sacks_suffered
                                                            
        public string FieldGoalsMade { get; set; } = string.Empty;    // fg_made_list
        public string FieldGoalsMissed { get; set; } = string.Empty;  // fg_missed_list
        public short ExtraPointsMade { get; set; } = 0;         // pat_made
        public short ExtraPointsAttempted { get; set; } = 0;    // pat_att
        public short PassingTwoPointConversions { get; set; } = 0;   // passing_2pt_conversions
        public short RushingTwoPointConversions { get; set; } = 0;   // rushing_2pt_conversions
        public short ReceivingTwoPointConversions { get; set; } = 0; // receiving_2pt_conversions
        public short ReturnedTouchdowns { get; set; } = 0;     // special_teams_tds
    }
}

/*
    R

    player_stats <- load_player_stats(2026)

    off_player_stats <- player_stats[
        player_stats$position %in% c("QB", "RB", "WR", "TE", "K", "P"),
    ]

    playerGame <- data.frame(
	    PlayerId = off_player_stats$player_id,
	    GameId = off_player_stats$game_id,
        Season = off_player_stats$season,
	    PassAttempts = off_player_stats$attempts,
	    PassCompletions = off_player_stats$completions,
	    PassingYards = off_player_stats$passing_yards,
	    PassingTouchdowns = off_player_stats$passing_tds,
	    RushAttempts = off_player_stats$carries,
	    RushingYards = off_player_stats$rushing_yards,
	    RushingFirstDowns = off_player_stats$rushing_first_downs,
	    RushingTouchdowns = off_player_stats$rushing_tds,
	    Receptions = off_player_stats$receptions,
	    ReceivingYards = off_player_stats$receiving_yards,
	    ReceivingTouchdowns = off_player_stats$receiving_tds,
	    Interceptions = off_player_stats$passing_interceptions,
	    Fumbles = off_player_stats$fumbles_lost_total,
	    Sacks = off_player_stats$sacks_suffered,
	    FieldGoalsMade = off_player_stats$fg_made_list,
	    FieldGoalsMissed = off_player_stats$fg_missed_list,
	    ExtraPointsMade = off_player_stats$pat_made,
	    ExtraPointsAttempted = off_player_stats$pat_att,
	    PassingTwoPointConversions = off_player_stats$passing_2pt_conversions,
	    RushingTwoPointConversions = off_player_stats$rushing_2pt_conversions,
	    ReceivingTwoPointConversions = off_player_stats$receiving_2pt_conversions,
	    ReturnedTouchdowns = off_player_stats$special_teams_tds
    )
 */
