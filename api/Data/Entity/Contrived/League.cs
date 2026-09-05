using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api.Data.Entity.Contrived;

[Index(nameof(JoinCode), IsUnique = true)]
public class League
{
    [Key]
    public int LeagueId { get; init; }

    [MaxLength(8)]
    public required string JoinCode { get; init; }

    [MaxLength(50)]
    public required string Name { get; init; }

    public ICollection<LeagueMember> Members { get; init; } = [];
    public required Settings Settings { get; init; }
    public ICollection<Team> Teams { get; init; } = [];
}
