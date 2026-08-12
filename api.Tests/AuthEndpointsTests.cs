using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FootballGm.Api.Tests;

public class AuthEndpointsTests : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _client;

    public AuthEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_returns_tokens_and_me_works()
    {
        var email = UniqueEmail();
        var auth = await RegisterAsync(email, "correct-horse-battery", "Nick");

        Assert.False(string.IsNullOrWhiteSpace(auth.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(auth.RefreshToken));
        Assert.Equal(email, auth.User.Email);
        Assert.Equal("Nick", auth.User.DisplayName);

        var me = await GetMeAsync(auth.AccessToken);
        Assert.Equal(auth.User.Id, me.Id);
        Assert.Equal(email, me.Email);
    }

    [Fact]
    public async Task Register_duplicate_email_returns_conflict()
    {
        var email = UniqueEmail();
        await RegisterAsync(email, "correct-horse-battery", "Nick");

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email, password = "correct-horse-battery", displayName = "Other" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_success_and_bad_password()
    {
        var email = UniqueEmail();
        await RegisterAsync(email, "correct-horse-battery", "Nick");

        var login = await LoginAsync(email, "correct-horse-battery");
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(login.RefreshToken));

        var bad = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password = "wrong-password-here" });

        Assert.Equal(HttpStatusCode.Unauthorized, bad.StatusCode);
    }

    [Fact]
    public async Task Me_without_token_returns_unauthorized()
    {
        var response = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_rotates_and_rejects_reuse()
    {
        var email = UniqueEmail();
        var auth = await RegisterAsync(email, "correct-horse-battery", "Nick");
        var oldRefresh = auth.RefreshToken;

        var refreshed = await RefreshAsync(oldRefresh);
        Assert.False(string.IsNullOrWhiteSpace(refreshed.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(refreshed.RefreshToken));
        Assert.NotEqual(oldRefresh, refreshed.RefreshToken);

        var reuse = await _client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = oldRefresh });

        Assert.Equal(HttpStatusCode.Unauthorized, reuse.StatusCode);

        // New access token still works.
        var me = await GetMeAsync(refreshed.AccessToken);
        Assert.Equal(email, me.Email);
    }

    [Fact]
    public async Task Logout_then_refresh_fails()
    {
        var email = UniqueEmail();
        var auth = await RegisterAsync(email, "correct-horse-battery", "Nick");

        var logout = await _client.PostAsJsonAsync(
            "/api/auth/logout",
            new { refreshToken = auth.RefreshToken });

        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        var refresh = await _client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = auth.RefreshToken });

        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    [Fact]
    public async Task Change_password_revokes_refresh_and_requires_new_login()
    {
        var email = UniqueEmail();
        const string oldPassword = "correct-horse-battery";
        const string newPassword = "new-correct-horse";

        var auth = await RegisterAsync(email, oldPassword, "Nick");
        var oldRefresh = auth.RefreshToken;

        using var changeRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/change-password");
        changeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        changeRequest.Content = JsonContent.Create(new
        {
            currentPassword = oldPassword,
            newPassword,
        });

        var changeResponse = await _client.SendAsync(changeRequest);
        Assert.Equal(HttpStatusCode.NoContent, changeResponse.StatusCode);

        var refresh = await _client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken = oldRefresh });
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);

        var oldLogin = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password = oldPassword });
        Assert.Equal(HttpStatusCode.Unauthorized, oldLogin.StatusCode);

        var newLogin = await LoginAsync(email, newPassword);
        Assert.False(string.IsNullOrWhiteSpace(newLogin.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(newLogin.RefreshToken));
    }

    [Fact]
    public async Task Change_password_wrong_current_returns_unauthorized()
    {
        var email = UniqueEmail();
        var auth = await RegisterAsync(email, "correct-horse-battery", "Nick");

        using var changeRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/change-password");
        changeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        changeRequest.Content = JsonContent.Create(new
        {
            currentPassword = "not-the-right-password",
            newPassword = "new-correct-horse",
        });

        var changeResponse = await _client.SendAsync(changeRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, changeResponse.StatusCode);
    }

    [Fact]
    public async Task Health_is_anonymous_and_healthy()
    {
        var response = await _client.GetAsync("/api/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.True(body!.DatabaseConnected);
    }

    private async Task<AuthResponseDto> RegisterAsync(string email, string password, string displayName)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email, password, displayName, deviceName = "tests" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions);
        Assert.NotNull(auth);
        return auth!;
    }

    private async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password, deviceName = "tests" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions);
        Assert.NotNull(auth);
        return auth!;
    }

    private async Task<AuthResponseDto> RefreshAsync(string refreshToken)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/refresh",
            new { refreshToken });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions);
        Assert.NotNull(auth);
        return auth!;
    }

    private async Task<UserDto> GetMeAsync(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var me = await response.Content.ReadFromJsonAsync<UserDto>(JsonOptions);
        Assert.NotNull(me);
        return me!;
    }

    private static string UniqueEmail() =>
        $"user-{Guid.NewGuid():N}@example.com";

    private sealed record AuthResponseDto(
        string AccessToken,
        string TokenType,
        DateTimeOffset ExpiresAt,
        string RefreshToken,
        DateTimeOffset RefreshExpiresAt,
        UserDto User);

    private sealed record UserDto(string Id, string Email, string DisplayName);

    private sealed record HealthDto(string Status, DateTimeOffset Timestamp, bool DatabaseConnected);
}
