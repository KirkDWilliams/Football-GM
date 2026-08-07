namespace FootballGm.Api.Data.Entity
{
    public class PlayerSeasonStats
    {
        public int PlayerId { get; set; }
        public short Season { get; set; }

        public short PassAttempts { get; set; }
        public short PassCompletions { get; set; }
        public short PassingYards { get; set; }
        public short PassingTouchdowns { get; set; }

        public short RushAttempts { get; set; }
        public short RushingYards { get; set; }
        public short RushingFirstDowns { get; set; }
        public short RushingTouchdowns { get; set; }

        public short Receptions { get; set; }
        public short ReceivingYards { get; set; }
        public short ReceivingTouchdowns { get; set; }


        public short Interceptions { get; set; }
        public short Fumbles { get; set; }
        public short Sacks { get; set; }

        public short FieldGoalsMade { get; set; }
        public short FieldGoalsAttempted { get; set; }
        public short ExtraPointsMade { get; set; }
        public short ExtraPointsAttempted { get; set; }

        public short ReturnedTouchdowns { get; set; }
    }
}
