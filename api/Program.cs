using FootballGm.Api.Auth;
using FootballGm.Api.Data;
using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Infrastructure;
using FootballGm.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;

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
            Description = "JWT from POST /api/auth/login (or register). Paste the accessToken value.",
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
    ?? throw new InvalidOperationException($"Configuration section '{JwtOptions.SectionName}' is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey) || jwtOptions.SigningKey.Length < 32)
{
    throw new InvalidOperationException(
        $"{JwtOptions.SectionName}:SigningKey must be configured and at least 32 characters.");
}

if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
{
    throw new InvalidOperationException(
        $"{JwtOptions.SectionName}:Issuer and Audience must be configured.");
}

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IRefreshTokenMaintenance, RefreshTokenMaintenance>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<RefreshTokenCleanupHostedService>();
builder.Services.AddScoped<PlayerRepository>();

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
            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FlutterDev", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin))
                {
                    return false;
                }

                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                // Allow local Flutter web / desktop / tooling origins during development.
                return uri.Host is "localhost" or "127.0.0.1" or "10.0.2.2";
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply pending EF Core migrations (creates the SQLite file when missing).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("FlutterDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
