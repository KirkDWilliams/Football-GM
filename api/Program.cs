using System.Security.Claims;
using System.Text;
using FootballGm.Api.Auth;
using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Domain;
using FootballGm.Api.Domain.Interfaces;
using FootballGm.Api.Infrastructure;
using FootballGm.Api.Infrastructure.Interfaces;
using FootballGm.Api.Services;
using FootballGm.Api.Services.GameAnalysis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT from POST /api/auth/login (or register). Paste the accessToken value."
        };
        return Task.CompletedTask;
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=footballgm.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.Configure<JwtOptions>(jwtSection);

var jwtOptions = jwtSection.Get<JwtOptions>()
                 ?? throw new InvalidOperationException(
                     $"Configuration section '{JwtOptions.SectionName}' is missing.");

ValidateJwtOptions(jwtOptions, builder.Environment);

var corsSection = builder.Configuration.GetSection(CorsOptions.SectionName);
builder.Services.Configure<CorsOptions>(corsSection);
var corsOptions = corsSection.Get<CorsOptions>() ?? new CorsOptions();

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IRefreshTokenMaintenance, RefreshTokenMaintenance>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<RefreshTokenCleanupHostedService>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IScoreCalculator, ScoreCalculator>();
builder.Services.AddScoped<IPlayerOrchestrator, PlayerOrchestrator>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<ILeagueCodeService, LeagueCodeService>();
builder.Services.AddScoped<ILeagueOrchestrator, LeagueOrchestrator>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<ITeamOrchestrator, TeamOrchestrator>();
builder.Services.AddScoped<IContractOrchestrator, ContractOrchestrator>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

const string corsPolicyName = "AppCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        // Local Flutter web / emulator tooling.
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
        {
            policy
                .SetIsOriginAllowed(IsLocalFlutterOrigin)
                .AllowAnyHeader()
                .AllowAnyMethod();
            return;
        }

        // Production / Staging: explicit allow-list only (env: Cors__AllowedOrigins__0=...).
        var origins = corsOptions.AllowedOrigins
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .Select(o => o.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (origins.Length == 0)
        {
            // No browser clients configured — reject all browser origins.
            policy.SetIsOriginAllowed(_ => false);
            return;
        }

        policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply schema: migrations for real environments; EnsureCreated for integration tests
// (migration history was evolved against an existing DB and is not empty-DB safe yet).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (app.Environment.IsEnvironment("Testing"))
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();

    if (app.Environment.IsDevelopment())
    {
        var dataSource = connectionString
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault(part => part.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));

        if (dataSource is not null)
        {
            var relativePath = dataSource["Data Source=".Length..].Trim();
            var absolutePath = Path.GetFullPath(relativePath);
            app.Logger.LogInformation("SQLite database path: {DatabasePath}", absolutePath);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Prefer HTTP in local Flutter scenarios (Android emulator, web). HTTPS can be enabled via the https launch profile.
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();

app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static void ValidateJwtOptions(JwtOptions jwtOptions, IHostEnvironment environment)
{
    if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey) || jwtOptions.SigningKey.Length < 32)
        throw new InvalidOperationException(
            $"{JwtOptions.SectionName}:SigningKey must be configured and at least 32 characters. " +
            "Set Jwt__SigningKey via environment variables or user secrets for non-local deploys.");

    if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
        throw new InvalidOperationException(
            $"{JwtOptions.SectionName}:Issuer and Audience must be configured.");

    // Development and Testing may use appsettings keys; never ship the placeholder.
    if (environment.IsDevelopment() || environment.IsEnvironment("Testing")) return;

    if (IsInsecureJwtSigningKey(jwtOptions.SigningKey))
        throw new InvalidOperationException(
            $"{JwtOptions.SectionName}:SigningKey is missing or still set to the placeholder. " +
            "For Production/Staging set a long random secret via environment variable Jwt__SigningKey " +
            "(or user secrets / a secret store). Do not commit real signing keys.");
}

static bool IsInsecureJwtSigningKey(string signingKey)
{
    if (string.IsNullOrWhiteSpace(signingKey)) return true;

    // Matches appsettings.json placeholder and similar "replace me" values.
    if (signingKey.Contains("REPLACE_WITH", StringComparison.OrdinalIgnoreCase)
        || signingKey.Contains("CHANGE_ME", StringComparison.OrdinalIgnoreCase)
        || signingKey.Contains("TODO", StringComparison.OrdinalIgnoreCase))
        return true;

    return false;
}

static bool IsLocalFlutterOrigin(string? origin)
{
    if (string.IsNullOrWhiteSpace(origin)) return false;

    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;

    return uri.Host is "localhost" or "127.0.0.1" or "10.0.2.2";
}

public partial class Program;
