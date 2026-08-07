namespace FootballGm.Api.Data.Entity
{
    public class Player
    {
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
        public short JerseyNumber { get; set; }
        public short Age { get; set; }
        public short DraftYear { get; set; }
        public bool IsInjured { get; set; } = false;
    }
}
