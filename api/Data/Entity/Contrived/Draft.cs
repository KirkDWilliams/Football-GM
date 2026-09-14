using System.ComponentModel.DataAnnotations;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Entity.Contrived;

public class Draft
{
    [Key]
    public int Id { get; set; }

    public int LeagueId { get; set; }

    public DraftStatus Status { get; set; }
}
