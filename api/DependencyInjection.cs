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
using FootballGm.Api.Services.BackgroundServices;
using FootballGm.Api.Services.GameAnalysis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace FootballGm.Api;

public static class DependencyInjection
{
    public const string CorsPolicyName = "AppCors";

    public static void AddFootballGmServices(this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddControllers();
        AddOpenApi(services);
        AddDatabase(services, configuration);

        var jwtOptions = BindAndValidateJwt(services, configuration, environment);
        AddAuthentication(services, jwtOptions);
        services.AddAuthorization();

        AddCors(services, configuration, environment);
        AddApplicationServices(services);
        services.AddSignalR();
    }

    private static void AddOpenApi(IServiceCollection services)
    {
        services.AddOpenApi(options =>
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
    }

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Data Source=footballgm.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));
    }

    private static JwtOptions BindAndValidateJwt(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        services.Configure<JwtOptions>(jwtSection);

        var jwtOptions = jwtSection.Get<JwtOptions>()
                         ?? throw new InvalidOperationException(
                             $"Configuration section '{JwtOptions.SectionName}' is missing.");

        ValidateJwtOptions(jwtOptions, environment);
        return jwtOptions;
    }

    private static void AddAuthentication(IServiceCollection services, JwtOptions jwtOptions)
    {
        services
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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private static void AddCors(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var corsSection = configuration.GetSection(CorsOptions.SectionName);
        services.Configure<CorsOptions>(corsSection);
        var corsOptions = corsSection.Get<CorsOptions>() ?? new CorsOptions();

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                // Local Flutter web / emulator tooling.
                if (environment.IsDevelopment() || environment.IsEnvironment("Testing"))
                {
                    policy
                        .SetIsOriginAllowed(IsLocalFlutterOrigin)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
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
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IRefreshTokenMaintenance, RefreshTokenMaintenance>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IScoreCalculator, ScoreCalculator>();
        services.AddScoped<IPlayerOrchestrator, PlayerOrchestrator>();

        services.AddScoped<ILeagueRepository, LeagueRepository>();
        services.AddScoped<ILeagueCodeService, LeagueCodeService>();
        services.AddScoped<ILeagueOrchestrator, LeagueOrchestrator>();

        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<ITeamOrchestrator, TeamOrchestrator>();
        services.AddScoped<IContractOrchestrator, ContractOrchestrator>();

        services.AddHostedService<RefreshTokenCleanupHostedService>();
        services.AddHostedService<MasterBackgroundService>();
    }

    private static void ValidateJwtOptions(JwtOptions jwtOptions, IHostEnvironment environment)
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

    private static bool IsInsecureJwtSigningKey(string signingKey)
    {
        if (string.IsNullOrWhiteSpace(signingKey)) return true;

        // Matches appsettings.json placeholder and similar "replace me" values.
        if (signingKey.Contains("REPLACE_WITH", StringComparison.OrdinalIgnoreCase)
            || signingKey.Contains("CHANGE_ME", StringComparison.OrdinalIgnoreCase)
            || signingKey.Contains("TODO", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static bool IsLocalFlutterOrigin(string? origin)
    {
        if (string.IsNullOrWhiteSpace(origin)) return false;

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;

        return uri.Host is "localhost" or "127.0.0.1" or "10.0.2.2";
    }
}
