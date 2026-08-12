using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace FootballGm.Api.Tests;

/// <summary>
/// Boots the real API with an isolated SQLite file and test JWT settings.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(
        Path.GetTempPath(),
        $"footballgm-test-{Guid.NewGuid():N}.db");

    private Dictionary<string, string?> TestSettings => new()
    {
        ["ConnectionStrings:DefaultConnection"] = $"Data Source={_dbPath}",
        ["Jwt:Issuer"] = "FootballGm.Api.Tests",
        ["Jwt:Audience"] = "FootballGm.Client.Tests",
        ["Jwt:SigningKey"] = "test-signing-key-at-least-32-characters-long!!",
        ["Jwt:ExpirationMinutes"] = "30",
        ["Jwt:RefreshTokenExpirationDays"] = "30",
        ["Jwt:MaxActiveRefreshTokensPerUser"] = "10",
        ["Jwt:RefreshTokenCleanupRetentionDays"] = "7",
        ["Jwt:RefreshTokenCleanupIntervalHours"] = "24",
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Testing uses EnsureCreated (see Program.cs) so we avoid broken empty-DB migrations.
        builder.UseEnvironment("Testing");

        // UseSetting is applied early and reliably for WebApplicationFactory hosts.
        foreach (var pair in TestSettings)
        {
            builder.UseSetting(pair.Key, pair.Value);
        }

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(TestSettings);
        });

        builder.ConfigureTestServices(services =>
        {
            // Hosted cleanup is not needed for these tests and can race with teardown.
            services.RemoveAll<IHostedService>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Host configuration is available before Program service registration reads Jwt / connection strings.
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(TestSettings);
        });

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        TryDelete(_dbPath);
        TryDelete(_dbPath + "-shm");
        TryDelete(_dbPath + "-wal");
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best-effort temp cleanup.
        }
    }
}
