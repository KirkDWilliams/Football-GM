namespace FootballGm.Api.Data.Entity
{
    public class PlayerSeason 
    {
        public string PlayerId { get; set; } = string.Empty;
        public short Season { get; set; } = 0;               
        public short PassAttempts { get; set; } = 0;
        public short PassCompletions { get; set; } = 0;
        public short PassingYards { get; set; } = 0;
        public short PassingTouchdowns { get; set; } = 0;

        public short RushAttempts { get; set; } = 0;
        public short RushingYards { get; set; } = 0;
        public short RushingBrokenTackles { get; set; } = 0;
        public short RushingFirstDowns { get; set; } = 0;
        public short RushingTouchdowns { get; set; } = 0;

        public short Receptions { get; set; } = 0;
        public short ReceivingYards { get; set; } = 0;
        public short ReceivingTouchdowns { get; set; } = 0;



        public short Interceptions { get; set; } = 0;
        public short Fumbles { get; set; } = 0;
        public short Sacks { get; set; } = 0;

        public string FieldGoalsMade { get; set; } = string.Empty;
        public string FieldGoalsMissed { get; set; } = string.Empty;
        public short ExtraPointsMade { get; set; } = 0;
        public short ExtraPointsAttempted { get; set; } = 0;

        public short PassingTwoPointConversions { get; set; } = 0;
        public short TwoPointConversionsMade { get; set; } = 0;   
        public short ReturnedTouchdowns { get; set; }
    }
}

/*
 * R
 
     playerSeason <- playerGame %>%
        group_by(Season, PlayerId) %>%
        summarise(
            PassAttempts = sum(PassAttempts, na.rm = TRUE),
            PassCompletions = sum(PassCompletions, na.rm = TRUE),
            PassingYards = sum(PassingYards, na.rm = TRUE),
            PassingTouchdowns = sum(PassingTouchdowns, na.rm = TRUE),
            RushAttempts = sum(RushAttempts, na.rm = TRUE),
            RushingYards = sum(RushingYards, na.rm = TRUE),
            RushingFirstDowns = sum(RushingFirstDowns, na.rm = TRUE),
            RushingTouchdowns = sum(RushingTouchdowns, na.rm = TRUE),
            Receptions = sum(Receptions, na.rm = TRUE),
            ReceivingYards = sum(ReceivingYards, na.rm = TRUE),
            ReceivingTouchdowns = sum(ReceivingTouchdowns, na.rm = TRUE),
            Interceptions = sum(Interceptions, na.rm = TRUE),
            Fumbles = sum(Fumbles, na.rm = TRUE),
            Sacks = sum(Sacks, na.rm = TRUE),
            FieldGoalsMade = paste(
                FieldGoalsMade[!is.na(FieldGoalsMade) & FieldGoalsMade != ""],
                collapse = ","
            ),
            FieldGoalsMissed = paste(
                FieldGoalsMissed[!is.na(FieldGoalsMissed) & FieldGoalsMissed != ""],
                collapse = ","
            ),
            ExtraPointsMade = sum(ExtraPointsMade, na.rm = TRUE),
            ExtraPointsAttempted = sum(ExtraPointsAttempted, na.rm = TRUE),
            PassingTwoPointConversions = sum(PassingTwoPointConversions, na.rm = TRUE),
            RushingTwoPointConversions = sum(RushingTwoPointConversions, na.rm = TRUE),
            ReceivingTwoPointConversions = sum(ReceivingTwoPointConversions, na.rm = TRUE),
            ReturnedTouchdowns = sum(ReturnedTouchdowns, na.rm = TRUE),
            .groups = "drop"
        )
 */
