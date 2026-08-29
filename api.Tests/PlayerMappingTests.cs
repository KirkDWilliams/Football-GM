using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Tests;

public class PlayerMappingTests
{
    [Theory]
    [InlineData("BUF", NflTeam.BuffaloBills)]
    [InlineData("MIA", NflTeam.MiamiDolphins)]
    [InlineData("NE", NflTeam.NewEnglandPatriots)]
    [InlineData("NYJ", NflTeam.NewYorkJets)]
    [InlineData("BAL", NflTeam.BaltimoreRavens)]
    [InlineData("CIN", NflTeam.CincinnatiBengals)]
    [InlineData("CLE", NflTeam.ClevelandBrowns)]
    [InlineData("PIT", NflTeam.PittsburghSteelers)]
    [InlineData("HOU", NflTeam.HoustonTexans)]
    [InlineData("IND", NflTeam.IndianapolisColts)]
    [InlineData("JAX", NflTeam.JacksonvilleJaguars)]
    [InlineData("TEN", NflTeam.TennesseeTitans)]
    [InlineData("DEN", NflTeam.DenverBroncos)]
    [InlineData("KC", NflTeam.KansasCityChiefs)]
    [InlineData("LV", NflTeam.LasVegasRaiders)]
    [InlineData("LAC", NflTeam.LosAngelesChargers)]
    [InlineData("DAL", NflTeam.DallasCowboys)]
    [InlineData("NYG", NflTeam.NewYorkGiants)]
    [InlineData("PHI", NflTeam.PhiladelphiaEagles)]
    [InlineData("WAS", NflTeam.WashingtonCommanders)]
    [InlineData("CHI", NflTeam.ChicagoBears)]
    [InlineData("DET", NflTeam.DetroitLions)]
    [InlineData("GB", NflTeam.GreenBayPackers)]
    [InlineData("MIN", NflTeam.MinnesotaVikings)]
    [InlineData("ATL", NflTeam.AtlantaFalcons)]
    [InlineData("CAR", NflTeam.CarolinePanthers)]
    [InlineData("NO", NflTeam.NewOrleansSaints)]
    [InlineData("TB", NflTeam.TampaBayBuccaneers)]
    [InlineData("ARI", NflTeam.ArizonaCardinals)]
    [InlineData("LA", NflTeam.LosAngelesRams)]
    [InlineData("SF", NflTeam.SanFrancisco49ers)]
    [InlineData("SEA", NflTeam.SeattleSeahawks)]
    public void ParseNflTeam_maps_current_nflverse_abbreviations(string abbreviation, NflTeam expected)
    {
        Assert.Equal(expected, Player.ParseNflTeam(abbreviation));
    }

    [Theory]
    [InlineData("nwe", NflTeam.NewEnglandPatriots)]
    [InlineData("jac", NflTeam.JacksonvilleJaguars)]
    [InlineData("kan", NflTeam.KansasCityChiefs)]
    [InlineData("oak", NflTeam.LasVegasRaiders)]
    [InlineData("sd", NflTeam.LosAngelesChargers)]
    [InlineData("wsh", NflTeam.WashingtonCommanders)]
    [InlineData("gnb", NflTeam.GreenBayPackers)]
    [InlineData("nor", NflTeam.NewOrleansSaints)]
    [InlineData("tam", NflTeam.TampaBayBuccaneers)]
    [InlineData("lar", NflTeam.LosAngelesRams)]
    [InlineData("sfo", NflTeam.SanFrancisco49ers)]
    [InlineData(" kc ", NflTeam.KansasCityChiefs)]
    public void ParseNflTeam_maps_aliases_and_ignores_case(string abbreviation, NflTeam expected)
    {
        Assert.Equal(expected, Player.ParseNflTeam(abbreviation));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParseNflTeam_requires_a_value(string? abbreviation)
    {
        Assert.Throws<ArgumentException>(() => Player.ParseNflTeam(abbreviation!));
    }

    [Fact]
    public void ParseNflTeam_rejects_unknown_abbreviations()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Player.ParseNflTeam("XYZ"));
    }

    [Theory]
    [InlineData("QB", Position.Quarterback)]
    [InlineData("RB", Position.RunningBack)]
    [InlineData("WR", Position.WideReceiver)]
    [InlineData("TE", Position.TightEnd)]
    [InlineData("K", Position.Kicker)]
    [InlineData("P", Position.Punter)]
    [InlineData("fb", Position.RunningBack)]
    [InlineData(" pk ", Position.Kicker)]
    public void ParsePosition_maps_nflverse_codes(string code, Position expected)
    {
        Assert.Equal(expected, Player.ParsePosition(code));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ParsePosition_requires_a_value(string? code)
    {
        Assert.Throws<ArgumentException>(() => Player.ParsePosition(code!));
    }

    [Fact]
    public void ParsePosition_rejects_unknown_codes()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Player.ParsePosition("OL"));
    }

    [Fact]
    public void FromEntity_parses_nflverse_team_and_position()
    {
        var entity = new Data.Entity.Ingested.Player
        {
            PlayerId = "00-0033873",
            Name = "Patrick Mahomes",
            Team = "KC",
            Position = "QB",
            JerseyNumber = 15,
            DraftYear = 2017
        };

        var player = Player.FromEntity(entity);

        Assert.Equal("00-0033873", player.Id);
        Assert.Equal("Patrick Mahomes", player.Name);
        Assert.Equal(NflTeam.KansasCityChiefs, player.Team);
        Assert.Equal(Position.Quarterback, player.Position);
        Assert.Equal(15, player.JerseyNumber);
        Assert.Equal(2017, player.DraftYear);
        Assert.Empty(player.Stats);
    }

    [Fact]
    public void Player_can_carry_one_or_more_stat_sets_without_duplicated_score_fields()
    {
        var player = Player.FromEntity(new Data.Entity.Ingested.Player
        {
            PlayerId = "00-0033873",
            Name = "Patrick Mahomes",
            Team = "KC",
            Position = "QB"
        });

        var previousWeek = StatSet.From(
            StatSetKind.PreviousWeek,
            [new StatScore { StatType = StatType.PassingYards, Value = 12m }]);
        var season = StatSet.From(
            StatSetKind.Season,
            [new StatScore { StatType = StatType.PassingYards, Value = 80m }]);

        var withOneSet = player with { Stats = [previousWeek] };
        var withTwoSets = player with { Stats = [previousWeek, season] };

        Assert.Equal(12m, previousWeek.Total);
        Assert.Single(withOneSet.Stats);
        Assert.Equal(StatSetKind.PreviousWeek, withOneSet.Stats[0].Kind);
        Assert.Equal(2, withTwoSets.Stats.Count);
        Assert.Equal(80m, withTwoSets.Stats[1].Total);
    }
}
