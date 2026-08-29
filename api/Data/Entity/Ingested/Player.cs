using System.ComponentModel.DataAnnotations;
using FootballGm.Api.Data.Entity.Associations;

namespace FootballGm.Api.Data.Entity.Ingested;

public class Player
{
    [Key]
    public string PlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public short JerseyNumber { get; set; }
    public short DraftYear { get; set; }

    public ICollection<TeamPlayers> TeamPlayers { get; set; } = [];
}
