namespace FootballGm.Api.Data.Entity
{
    public class Game
    {
        public int GameId { get; set; }
        public int Season { get; set; }

        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }

        public int WinningTeamId { get; set; }

        public short WinningScore { get; set; }
        public short LosingScore { get; set; }
    }
}
