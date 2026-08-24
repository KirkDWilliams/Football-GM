using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure.Interfaces;
using Entities = FootballGm.Api.Data.Entity.Contrived;

namespace FootballGm.Api.Domain;

public interface ILeagueOrchestrator
{
    Task<League> CreateLeague(string userId, League league, CancellationToken cancellationToken);
}

public class LeagueOrchestrator(ILeagueRepository repository) : ILeagueOrchestrator
{
    public async Task<League> CreateLeague(
        string userId,
        League league,
        CancellationToken cancellationToken)
    {
        var entity = new Entities.League
        {
            AdminUserId = userId,
            Name = league.Name.Trim(),
            Settings = new Entities.Settings
            {
                WeeklyCapSpace = league.WeeklyCapSpace,
                EligiblePositions = [.. league.Positions.Distinct()],
                Rules = ToEntityRules(league.Rules)
            }
        };

        var saved = await repository.AddAsync(entity, cancellationToken);

        return new League(saved.Name, league.Positions, league.Rules)
        {
            LeagueId = saved.LeagueId,
            WeeklyCapSpace = saved.Settings.WeeklyCapSpace
        };
    }

    private static List<Entities.Rule> ToEntityRules(IEnumerable<Rule> rules) =>
    [
        .. rules.Select<Rule, Entities.Rule>(rule => rule switch
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
