using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FootballGm.Api.Data.Models;

namespace FootballGm.Api.Tests;

public class LeagueEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_league_without_token_returns_unauthorized()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/league",
            new { name = "Sunday League" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_league_without_name_returns_bad_request()
    {
        var auth = await RegisterAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/league");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        request.Content = JsonContent.Create(new { name = "  " });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_league_uses_default_scoring_weights()
    {
        var auth = await RegisterAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/league");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        request.Content = JsonContent.Create(new { name = "Sunday League" });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<League>(JsonOptions);
        Assert.NotNull(created);
        Assert.True(created.LeagueId > 0);
        Assert.Equal("Sunday League", created.Name);
        Assert.NotEmpty(created.Rules);
        Assert.All(created.Rules, rule => Assert.IsType<ScoringWeightRule>(rule));
        Assert.Equal(Rule.CreateDefaultScoringWeights().Count, created.Rules.Count);
    }

    private async Task<AuthResponseDto> RegisterAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                email = $"user-{Guid.NewGuid():N}@example.com",
                password = "correct-horse-battery",
                displayName = "Nick",
                deviceName = "tests"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions);
        Assert.NotNull(auth);
        return auth!;
    }

    private sealed record AuthResponseDto(
        string AccessToken,
        string TokenType,
        DateTimeOffset ExpiresAt,
        string RefreshToken,
        DateTimeOffset RefreshExpiresAt,
        UserDto User);

    private sealed record UserDto(string Id, string Email, string DisplayName);
}
