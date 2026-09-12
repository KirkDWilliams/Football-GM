using FootballGm.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballGm.Api;

public static class DatabaseInitializer
{
    public static void InitializeDatabase(this WebApplication app)
    {
        // Apply schema: migrations for real environments; EnsureCreated for integration tests
        // (migration history was evolved against an existing DB and is not empty-DB safe yet).
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (app.Environment.IsEnvironment("Testing"))
            db.Database.EnsureCreated();
        else
            db.Database.Migrate();

        if (app.Environment.IsDevelopment())
            LogSqlitePath(app);
    }

    private static void LogSqlitePath(WebApplication app)
    {
        var connectionString = app.Configuration.GetConnectionString("DefaultConnection")
                               ?? "Data Source=footballgm.db";

        var dataSource = connectionString
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault(part => part.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));

        if (dataSource is null) return;

        var relativePath = dataSource["Data Source=".Length..].Trim();
        var absolutePath = Path.GetFullPath(relativePath);
        app.Logger.LogInformation("SQLite database path: {DatabasePath}", absolutePath);
    }
}
