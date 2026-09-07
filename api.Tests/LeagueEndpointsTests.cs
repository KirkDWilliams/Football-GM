using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FootballGm.Api.Data.Enums;
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
    public async Task Get_my_leagues_without_token_returns_unauthorized()
    {
        var response = await _client.GetAsync("/api/league");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_my_leagues_when_none_returns_empty_list()
    {
        var auth = await RegisterAsync();

        var response = await SendAuthorized(HttpMethod.Get, "/api/league", auth.AccessToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.Equal(0, doc.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task Get_my_leagues_returns_slim_summaries_only_for_memberships()
    {
        var commissioner = await RegisterAsync();
        var created = await CreateLeagueAsync(commissioner.AccessToken, "Sunday League");
        var stranger = await RegisterAsync();

        var mine = await SendAuthorized(HttpMethod.Get, "/api/league", commissioner.AccessToken);
        Assert.Equal(HttpStatusCode.OK, mine.StatusCode);

        using var mineDoc = JsonDocument.Parse(await mine.Content.ReadAsStringAsync());
        Assert.Equal(1, mineDoc.RootElement.GetArrayLength());
        var row = mineDoc.RootElement[0];
        Assert.Equal(created.LeagueId, row.GetProperty("leagueId").GetInt32());
        Assert.Equal("Sunday League", row.GetProperty("name").GetString());
        Assert.Equal(created.JoinCode, row.GetProperty("joinCode").GetString());
        Assert.Equal("commissioner", row.GetProperty("role").GetString());
        Assert.Equal("standard", row.GetProperty("scoring").GetString());
        Assert.False(row.TryGetProperty("rules", out _));
        Assert.False(row.TryGetProperty("positions", out _));
        Assert.False(row.TryGetProperty("weeklyCapSpace", out _));
        Assert.Equal(5, row.EnumerateObject().Count());

        var others = await SendAuthorized(HttpMethod.Get, "/api/league", stranger.AccessToken);
        Assert.Equal(HttpStatusCode.OK, others.StatusCode);
        using var othersDoc = JsonDocument.Parse(await others.Content.ReadAsStringAsync());
        Assert.Equal(0, othersDoc.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task Get_my_leagues_shows_member_role_after_join()
    {
        var commissioner = await RegisterAsync();
        var created = await CreateLeagueAsync(commissioner.AccessToken, "Sunday League");
        var joiner = await RegisterAsync();

        var joined = await SendAuthorized(
            HttpMethod.Post,
            $"/api/league/{created.JoinCode}",
            joiner.AccessToken);
        Assert.Equal(HttpStatusCode.Created, joined.StatusCode);

        var commissionerList = await SendAuthorized(HttpMethod.Get, "/api/league", commissioner.AccessToken);
        using var commissionerDoc = JsonDocument.Parse(await commissionerList.Content.ReadAsStringAsync());
        Assert.Equal("commissioner", commissionerDoc.RootElement[0].GetProperty("role").GetString());

        var joinerList = await SendAuthorized(HttpMethod.Get, "/api/league", joiner.AccessToken);
        using var joinerDoc = JsonDocument.Parse(await joinerList.Content.ReadAsStringAsync());
        Assert.Equal(1, joinerDoc.RootElement.GetArrayLength());
        Assert.Equal(created.LeagueId, joinerDoc.RootElement[0].GetProperty("leagueId").GetInt32());
        Assert.Equal("member", joinerDoc.RootElement[0].GetProperty("role").GetString());
        Assert.Equal("standard", joinerDoc.RootElement[0].GetProperty("scoring").GetString());
    }

    [Fact]
    public async Task Get_my_leagues_marks_custom_when_scoring_weights_differ()
    {
        var auth = await RegisterAsync();
        var rules = Rule.CreateDefaultScoringWeights();
        ((ScoringWeightRule)rules.First(rule => rule.Stat == StatType.PassingYards)).Weight = 0.05m;

        var created = await CreateLeagueAsync(auth.AccessToken, "Custom League", new { name = "Custom League", rules });
        Assert.Equal("Custom League", created.Name);

        var response = await SendAuthorized(HttpMethod.Get, "/api/league", auth.AccessToken);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("custom", doc.RootElement[0].GetProperty("scoring").GetString());
    }

    [Fact]
    public async Task Get_my_leagues_stays_standard_when_bonuses_or_positions_differ()
    {
        var auth = await RegisterAsync();

        var bonusRules = Rule.CreateDefaultScoringWeights();
        bonusRules.Add(new BonusRule { Stat = StatType.PassingYards, Threshold = 300, Points = 3 });
        await CreateLeagueAsync(
            auth.AccessToken,
            "Bonus League",
            new { name = "Bonus League", rules = bonusRules });

        await CreateLeagueAsync(
            auth.AccessToken,
            "Kicker League",
            new { name = "Kicker League", positions = new[] { Position.Kicker } });

        var response = await SendAuthorized(HttpMethod.Get, "/api/league", auth.AccessToken);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(2, doc.RootElement.GetArrayLength());
        Assert.All(
            doc.RootElement.EnumerateArray(),
            row => Assert.Equal("standard", row.GetProperty("scoring").GetString()));
    }

    [Fact]
    public async Task Get_league_without_token_returns_unauthorized()
    {
        var response = await _client.GetAsync("/api/league/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_league_returns_settings_and_role_for_member()
    {
        var commissioner = await RegisterAsync();
        var created = await CreateLeagueAsync(commissioner.AccessToken, "Sunday League");
        var joiner = await RegisterAsync();
        var joined = await SendAuthorized(
            HttpMethod.Post,
            $"/api/league/{created.JoinCode}",
            joiner.AccessToken);
        Assert.Equal(HttpStatusCode.Created, joined.StatusCode);

        var commissionerGet = await SendAuthorized(
            HttpMethod.Get,
            $"/api/league/{created.LeagueId}",
            commissioner.AccessToken);
        Assert.Equal(HttpStatusCode.OK, commissionerGet.StatusCode);
        using var commissionerDoc = JsonDocument.Parse(await commissionerGet.Content.ReadAsStringAsync());
        var commissionerLeague = commissionerDoc.RootElement;
        Assert.Equal(created.LeagueId, commissionerLeague.GetProperty("leagueId").GetInt32());
        Assert.Equal("Sunday League", commissionerLeague.GetProperty("name").GetString());
        Assert.Equal(created.JoinCode, commissionerLeague.GetProperty("joinCode").GetString());
        Assert.Equal(100m, commissionerLeague.GetProperty("weeklyCapSpace").GetDecimal());
        Assert.Equal("commissioner", commissionerLeague.GetProperty("role").GetString());
        Assert.Equal(Rule.CreateDefaultScoringWeights().Count, commissionerLeague.GetProperty("rules").GetArrayLength());
        Assert.True(commissionerLeague.GetProperty("positions").GetArrayLength() > 0);

        var joinerGet = await SendAuthorized(
            HttpMethod.Get,
            $"/api/league/{created.LeagueId}",
            joiner.AccessToken);
        Assert.Equal(HttpStatusCode.OK, joinerGet.StatusCode);
        using var joinerDoc = JsonDocument.Parse(await joinerGet.Content.ReadAsStringAsync());
        Assert.Equal("member", joinerDoc.RootElement.GetProperty("role").GetString());
        Assert.Equal(created.LeagueId, joinerDoc.RootElement.GetProperty("leagueId").GetInt32());
        Assert.Equal(100m, joinerDoc.RootElement.GetProperty("weeklyCapSpace").GetDecimal());
        Assert.True(joinerDoc.RootElement.GetProperty("rules").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Get_league_unknown_id_returns_not_found()
    {
        var auth = await RegisterAsync();

        var response = await SendAuthorized(HttpMethod.Get, "/api/league/999999", auth.AccessToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_league_for_non_member_returns_not_found()
    {
        var commissioner = await RegisterAsync();
        var created = await CreateLeagueAsync(commissioner.AccessToken, "Sunday League");
        var stranger = await RegisterAsync();

        var response = await SendAuthorized(
            HttpMethod.Get,
            $"/api/league/{created.LeagueId}",
            stranger.AccessToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

    private async Task<League> CreateLeagueAsync(string accessToken, string name, object? body = null)
    {
        var response = await SendAuthorized(
            HttpMethod.Post,
            "/api/league",
            accessToken,
            body ?? new { name });

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
