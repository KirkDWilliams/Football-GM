using FootballGm.Api.Data.Enums;

namespace FootballGm.Api.Data.Models;

public record Player
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public NflTeam Team { get; init; }
    public Position Position { get; init; }
    public short JerseyNumber { get; init; }
    public short DraftYear { get; init; }

    public IReadOnlyList<StatSet> Stats { get; init; } = [];

    public static Player FromEntity(Entity.Ingested.Player player) => new()
    {
        Id = player.PlayerId,
        Name = player.Name,
        Team = ParseNflTeam(player.Team),
        Position = ParsePosition(player.Position),
        JerseyNumber = player.JerseyNumber,
        DraftYear = player.DraftYear
    };

    public static NflTeam ParseNflTeam(string team)
    {
        if (string.IsNullOrWhiteSpace(team))
            throw new ArgumentException("Team abbreviation is required.", nameof(team));

        return team.Trim().ToUpperInvariant() switch
        {
            "BUF" => NflTeam.BuffaloBills,
            "MIA" => NflTeam.MiamiDolphins,
            "NE" or "NWE" => NflTeam.NewEnglandPatriots,
            "NYJ" => NflTeam.NewYorkJets,
            "BAL" => NflTeam.BaltimoreRavens,
            "CIN" => NflTeam.CincinnatiBengals,
            "CLE" => NflTeam.ClevelandBrowns,
            "PIT" => NflTeam.PittsburghSteelers,
            "HOU" => NflTeam.HoustonTexans,
            "IND" => NflTeam.IndianapolisColts,
            "JAX" or "JAC" => NflTeam.JacksonvilleJaguars,
            "TEN" => NflTeam.TennesseeTitans,
            "DEN" => NflTeam.DenverBroncos,
            "KC" or "KAN" => NflTeam.KansasCityChiefs,
            "LV" or "OAK" or "LVR" => NflTeam.LasVegasRaiders,
            "LAC" or "SD" => NflTeam.LosAngelesChargers,
            "DAL" => NflTeam.DallasCowboys,
            "NYG" => NflTeam.NewYorkGiants,
            "PHI" => NflTeam.PhiladelphiaEagles,
            "WAS" or "WSH" => NflTeam.WashingtonCommanders,
            "CHI" => NflTeam.ChicagoBears,
            "DET" => NflTeam.DetroitLions,
            "GB" or "GNB" => NflTeam.GreenBayPackers,
            "MIN" => NflTeam.MinnesotaVikings,
            "ATL" => NflTeam.AtlantaFalcons,
            "CAR" => NflTeam.CarolinePanthers,
            "NO" or "NOR" => NflTeam.NewOrleansSaints,
            "TB" or "TAM" => NflTeam.TampaBayBuccaneers,
            "ARI" => NflTeam.ArizonaCardinals,
            "LA" or "LAR" or "STL" => NflTeam.LosAngelesRams,
            "SF" or "SFO" => NflTeam.SanFrancisco49ers,
            "SEA" => NflTeam.SeattleSeahawks,
            _ => throw new ArgumentOutOfRangeException(
                nameof(team),
                team,
                "Unknown nflverse team abbreviation.")
        };
    }

    public static Position ParsePosition(string position)
    {
        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("Position is required.", nameof(position));

        return position.Trim().ToUpperInvariant() switch
        {
            "QB" => Position.Quarterback,
            "RB" or "FB" or "HB" => Position.RunningBack,
            "WR" => Position.WideReceiver,
            "TE" => Position.TightEnd,
            "K" or "PK" => Position.Kicker,
            "P" => Position.Punter,
            _ => throw new ArgumentOutOfRangeException(
                nameof(position),
                position,
                "Unknown nflverse position.")
        };
    }
}
