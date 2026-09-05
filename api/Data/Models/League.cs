using FootballGm.Api.Data.Enums;
using Entities = FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Data.Models;

public class League
{
    public int LeagueId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string JoinCode { get; init; } = string.Empty;
    public decimal WeeklyCapSpace { get; init; } = 100M;
    public List<Rule> Rules { get; init; } = Rule.CreateDefaultScoringWeights();
    public List<Position> Positions { get; init; } =
    [
        Position.Quarterback,
        Position.RunningBack,
        Position.RunningBack,
        Position.WideReceiver,
        Position.WideReceiver,
        Position.TightEnd,
        Position.Kicker
    ];

    public static League FromEntity(Entities.League entity) => new()
    {
        LeagueId = entity.LeagueId,
        Name = entity.Name,
        JoinCode = entity.JoinCode,
        WeeklyCapSpace = entity.Settings.WeeklyCapSpace,
        Positions = [.. entity.Settings.EligiblePositions],
        Rules = FromEntityRules(entity.Settings.Rules)
    };

    public Entities.League ToEntity(string joinCode, string commissionerUserId) => new()
    {
        JoinCode = joinCode,
        Name = Name.Trim(),
        Settings = new Entities.Settings
        {
            WeeklyCapSpace = WeeklyCapSpace,
            EligiblePositions = [.. Positions.Distinct()],
            Rules = ToEntityRules(Rules)
        },
        Members =
        [
            Entities.LeagueMember.Create(commissionerUserId, LeagueMemberRole.Commissioner)
        ]
    };

    private static List<Rule> FromEntityRules(IEnumerable<Entities.Rule> rules) =>
    [
        .. rules.Select<Entities.Rule, Rule>(rule => rule switch
        {
            Entities.ScoringWeightRule weight => new ScoringWeightRule
            {
                Stat = weight.Stat,
                Weight = weight.Weight
            },
            Entities.BonusRule bonus => new BonusRule
            {
                Stat = bonus.Stat,
                Threshold = bonus.Threshold,
                Points = bonus.Points
            },
            _ => throw new ArgumentOutOfRangeException(
                nameof(rules),
                rule.RuleType,
                "Unknown rule type.")
        })
    ];

    private static List<Entities.Rule> ToEntityRules(IReadOnlyCollection<Rule> rules)
    {
        var source = rules.Count > 0 ? rules : Rule.CreateDefaultScoringWeights();

        return
        [
            .. source.Select<Rule, Entities.Rule>(rule => rule switch
            {
                ScoringWeightRule weight => new Entities.ScoringWeightRule
                {
                    Stat = weight.Stat,
                    Weight = weight.Weight
                },
                BonusRule bonus => new Entities.BonusRule
                {
                    Stat = bonus.Stat,
                    Threshold = bonus.Threshold,
                    Points = bonus.Points
                },
                _ => throw new ArgumentOutOfRangeException(
                    nameof(rules),
                    rule.RuleType,
                    "Unknown rule type.")
            })
        ];
    }
}
