using FootballGm.Api.Data.Enums;
using FootballGm.Api.Domain.Interfaces;
using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Domain;

public interface ILeagueCodeService
{
    Task<string> GenerateUniqueJoinCodeAsync(CancellationToken cancellationToken);

    Task<bool> FixLeagueSettings(int leagueId, CancellationToken cancellationToken);
}

public class LeagueSetupService(
    ILeagueRepository repository,
    ILeagueOrchestrator leagueOrchestrator) : ILeagueCodeService
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

    // this needs to be called before the draft starts, after it is opened

    public async Task<bool> FixLeagueSettings(int leagueId, CancellationToken cancellationToken)
    {

        try
        {
            // determine the cap space off of the positions and the scoring rules
            var league = await repository.GetByIdAsync(leagueId, cancellationToken);
            if (league == null)
                return false; //TODO: error handling

            if (league.Settings.IsImmutable)
                return false; //TODO: already done handling

            CalculateLeagueCapSpace(league.Settings.EligiblePositions, league.Settings.Rules);

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

    private static float CalculateLeagueCapSpace(List<Position> positions, List<Data.Entity.Contrived.Rule> rules)
    {
        float value = 100f;
        // call the R script with the league specifics and run the averages of last year under that set of rules to determine the value per role and multiply it per positions on roster
        // the function will need to take in the positions value and quantity and the weightings per statline.

        return value;
    }
}
