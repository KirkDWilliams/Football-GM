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
        Assert.False(string.IsNullOrWhiteSpace(created.JoinCode));
    }

    [Fact]
    public async Task Join_league_without_token_returns_unauthorized()
    {
        var response = await _client.PostAsync("/api/league/ABCD1234", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Join_league_with_unknown_code_returns_not_found()
    {
        var auth = await RegisterAsync();

        var response = await SendAuthorized(HttpMethod.Post, "/api/league/NOPECODE", auth.AccessToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Join_league_adds_authenticated_user_as_member()
    {
        var commissioner = await RegisterAsync();
        var created = await CreateLeagueAsync(commissioner.AccessToken, "Sunday League");

        var joiner = await RegisterAsync();
        var response = await SendAuthorized(
            HttpMethod.Post,
            $"/api/league/{created.JoinCode}",
            joiner.AccessToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.EndsWith($"/api/league/{created.LeagueId}", response.Headers.Location?.ToString());

        var joined = await response.Content.ReadFromJsonAsync<League>(JsonOptions);
        Assert.NotNull(joined);
        Assert.Equal(created.LeagueId, joined.LeagueId);
        Assert.Equal("Sunday League", joined.Name);
        Assert.Equal(created.JoinCode, joined.JoinCode);
    }

    [Fact]
    public async Task Join_league_twice_returns_conflict()
    {
        var commissioner = await RegisterAsync();
        var created = await CreateLeagueAsync(commissioner.AccessToken, "Sunday League");

        var joiner = await RegisterAsync();
        var first = await SendAuthorized(
            HttpMethod.Post,
            $"/api/league/{created.JoinCode}",
            joiner.AccessToken);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await SendAuthorized(
            HttpMethod.Post,
            $"/api/league/{created.JoinCode}",
            joiner.AccessToken);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Join_league_as_creator_returns_conflict()
    {
        var commissioner = await RegisterAsync();
        var created = await CreateLeagueAsync(commissioner.AccessToken, "Sunday League");

        var response = await SendAuthorized(
            HttpMethod.Post,
            $"/api/league/{created.JoinCode}",
            commissioner.AccessToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private async Task<League> CreateLeagueAsync(string accessToken, string name)
    {
        var response = await SendAuthorized(
            HttpMethod.Post,
            "/api/league",
            accessToken,
            new { name });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<League>(JsonOptions);
        Assert.NotNull(created);
        return created;
    }

    private async Task<HttpResponseMessage> SendAuthorized(
        HttpMethod method,
        string url,
        string accessToken,
        object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        if (body is not null)
            request.Content = JsonContent.Create(body);

        return await _client.SendAsync(request);
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
