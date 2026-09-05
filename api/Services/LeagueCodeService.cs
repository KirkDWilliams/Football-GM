using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Services;

public interface ILeagueCodeService
{
    Task<string> GenerateUniqueJoinCodeAsync(CancellationToken cancellationToken);
}

public class LeagueCodeService(ILeagueRepository repository) : ILeagueCodeService
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

    private static string GenerateJoinCode() =>
        Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "")
            .Replace("/", "")
            .Replace("+", "")
            .Substring(0, 8);
}
