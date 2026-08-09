namespace FootballGm.Api.Data.Entity
{
    // load_player_stats(YEAR)
    // load_pfr_advstats(stat_type = 'RUSH, 'PASS', 'REC', summary_level = 'season')
    public class PlayerSeason 
    {
        public string PlayerId { get; set; } = string.Empty; // player_id
        public short Season { get; set; }                    // season
        public short PassAttempts { get; set; }        
        public short PassCompletions { get; set; }     
        public short PassingYards { get; set; }        
        public short PassingTouchdowns { get; set; }   

        public short RushAttempts { get; set; }        
        public short RushingYards { get; set; }        
        public short RushingBrokenTackles { get; set; }
        public short RushingFirstDowns { get; set; }   
        public short RushingTouchdowns { get; set; }   

        public short Receptions { get; set; }          
        public short ReceivingYards { get; set; }      
        public short ReceivingTouchdowns { get; set; } 



        public short Interceptions { get; set; }       
        public short Fumbles { get; set; }             
        public short Sacks { get; set; }               

        public string FieldGoalsMade { get; set; } = string.Empty;
        public string FieldGoalsMissed { get; set; } = string.Empty;
        public short ExtraPointsMade { get; set; } = 0;
        public short ExtraPointsAttempted { get; set; } = 0;

        public short PassingTwoPointConversions { get; set; } = 0;
        public short TwoPointConversionsMade { get; set; } = 0;   
        public short ReturnedTouchdowns { get; set; }
    }
}
