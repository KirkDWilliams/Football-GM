using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Infrastructure.Interfaces;
using static FootballGm.Api.Infrastructure.LeagueQueryExtensions;

namespace FootballGm.Api.Domain;

public interface ILeagueSetupService
{
    Task<string> GenerateUniqueJoinCodeAsync(CancellationToken cancellationToken);

    Task<bool> FixLeagueSettings(int leagueId, CancellationToken cancellationToken);
}

public class LeagueSetupService(ILeagueRepository repository) : ILeagueSetupService
{
    public async Task<string> GenerateUniqueJoinCodeAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var code = GenerateJoinCode();

            var exists = await repository.ExistsByJoinCodeAsync(code, cancellationToken);
            if (!exists)
                return code;
        }
    }

    public async Task<bool> FixLeagueSettings(int leagueId, CancellationToken cancellationToken)
    {
        try
        {
            // determine the cap space off of the positions and the scoring rules
            var league = await repository.GetByIdAsync(leagueId, LeagueIncludes.Settings, cancellationToken);
            if (league == null)
                return false; //TODO: error handling

            if (league.Settings.IsFixed)
                return false; //TODO: already done handling

            SetLeagueCapSpace(league.Settings.EligiblePositions, league.Settings.Rules);

            //await repository.UpdateAsync(league, cancellationToken);


            return true;
        }
        catch
        {
            return false;
        }

    }

    private static string GenerateJoinCode() =>
        Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "")
            .Replace("/", "")
            .Replace("+", "")
            .Substring(0, 8);

    private static float SetLeagueCapSpace(List<Position> positions, List<Data.Entity.Contrived.Rule> rules)
    {
        float value = 100f;
        var positionDict = Enum.GetValues<Position>().ToDictionary(stat => stat, _ => 0);

        foreach (var position in positions)
        {
            if (positionDict.TryGetValue(position, out int num))
                positionDict[position] = ++num;

            else if (!positionDict.TryAdd(position, 1))
               throw new AbandonedMutexException();
        }

        var positionsArray = positionDict.Values.ToArray();
        var scoringWeightRules = rules.OfType<ScoringWeightRule>().ToList();

        var statsDict = Enum.GetValues<StatType>().ToDictionary(stat => stat, _ => 0f);

        for (var r = 0; r < scoringWeightRules.Count; r++)
        {
            if (statsDict.ContainsKey(scoringWeightRules[r].Stat))
            {
                statsDict[scoringWeightRules[r].Stat] = scoringWeightRules[r].Weight;
            }
        }

        var weightsArray = statsDict.Values.ToArray();

        //var relativeWorthPerPosition = 

        //TODO: call the R script with the league specifics and run the averages of last year under that set of rules to determine the value per role and multiply it per positions on roster
        // the function will need to take in the positions value and quantity and the weightings per statline.

        return value;
    }
}
