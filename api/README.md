# API (`api/`)

ASP.NET Core 10 + SQLite + JWT auth.

```
api/
├── Controllers/          HTTP endpoints
├── Hubs/                 SignalR
├── Domain/               Orchestrators, helpers, scoring
├── Infrastructure/       Repositories + R ingestion scripts
├── Data/                 EF Core, entities, enums
├── Auth/                 JWT, identity, refresh-token maintenance
├── BackgroundServices/   Hosted jobs
├── Utility/              Shared helpers
└── Program.cs            App setup
```

```powershell
# from repo root
dotnet run --project api/FootballGm.Api.csproj --launch-profile http
dotnet test api.Tests/FootballGm.Api.Tests.csproj
```

- Dev: http://localhost:5000
- Scalar: http://localhost:5000/scalar/v1
- Samples: `FootballGm.Api.http`
