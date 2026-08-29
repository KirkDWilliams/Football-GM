using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FootballGm.Api.Tests;

public class PlayerEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_player_without_token_returns_unauthorized()
    {
        var response = await _client.GetAsync("/api/player/00-0033873?leagueId=1&gameId=2024_01_KC_BAL");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_player_without_game_id_returns_bad_request()
    {
        var auth = await RegisterAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/player/00-0033873?leagueId=1");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_unknown_player_returns_not_found()
    {
        var auth = await RegisterAsync();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/player/missing-player?leagueId=1&gameId=2024_01_KC_BAL");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
