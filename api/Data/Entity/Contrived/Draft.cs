using System.ComponentModel.DataAnnotations;
using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Entity.Contrived;

public class Draft
{
    [Key]
    public int Id { get; set; }

    public int LeagueId { get; set; }

    public DraftStatus Status { get; set; }

    [MaxLength(32)]
    public string? CurrentNominatorUserId { get; set; }

    public List<string> NominationOrder { get; set; } = [];
}
