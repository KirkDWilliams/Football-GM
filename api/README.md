# API (`api/`)

ASP.NET Core 10 + SQLite + JWT auth.

```
api/
├── Controllers/     HTTP endpoints
├── Services/        Auth + refresh-token maintenance
├── Auth/            JWT helpers, options
├── Data/            EF Core + entities
└── Program.cs       App setup
```

```powershell
# from repo root
dotnet run --project api/FootballGm.Api.csproj --launch-profile http
dotnet test tests/FootballGm.Api.Tests/FootballGm.Api.Tests.csproj
```

- Dev: http://localhost:5000  
- Scalar: http://localhost:5000/scalar/v1  
- Samples: `FootballGm.Api.http`
