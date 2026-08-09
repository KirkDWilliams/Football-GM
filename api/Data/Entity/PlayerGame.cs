namespace FootballGm.Api.Data.Entity
{
    public class PlayerGame
    {
        public string PlayerId { get; set; } = string.Empty; // player_id
        public string GameId { get; set; } = string.Empty;   // game_id

        public short PassAttempts { get; set; } = 0;        // attempts
        public short PassCompletions { get; set; } = 0;     // completions
        public short PassingYards { get; set; } = 0;        // passing_yards
        public short PassingTouchdowns { get; set; } = 0;

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
