# Football GM

A more complex take on fantasy football — think franchise GM decisions, not just weekly lineups. This monorepo hosts the **ASP.NET Core API** (SQLite) and the **Flutter** client.

## Stack

| Layer | Tech |
|-------|------|
| API | ASP.NET Core 10 (`net10.0`), Entity Framework Core, SQLite |
| Client | Flutter 3.44+ / Dart 3.12+ (Windows, Android, Web) |
| Local cache | Flutter `sqflite` (offline-friendly team cache) |

## Repository layout

```
Football-GM/
├── api/          # ASP.NET Core Web API (FootballGm.Api)
├── app/          # Flutter client (package: football_gm_app)
├── Football-GM.slnx
└── README.md
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Flutter](https://docs.flutter.dev/get-started/install) (stable channel)
- For Android: Android SDK / emulator
- For Windows desktop: Visual Studio with desktop C++ workload (Flutter Windows)

## Run the API

```powershell
dotnet restore api/FootballGm.Api.csproj
dotnet run --project api/FootballGm.Api.csproj --launch-profile http
```

| Resource | URL |
|----------|-----|
| HTTP base | `http://localhost:5000` |
| Health check | `http://localhost:5000/api/health` |
| OpenAPI document | `http://localhost:5000/openapi/v1.json` |
| Scalar UI (dev) | `http://localhost:5000/scalar/v1` |

SQLite file in Development: `footballgm.dev.db` (created next to the running process; gitignored).

Controller stubs exist for `Teams`, `Players`, `Leagues`, and `Games` (no business endpoints yet). Domain term **Game** is used instead of Match to align API and Flutter.

## Run the Flutter app

```powershell
cd app
flutter pub get
dart run build_runner build --delete-conflicting-outputs
flutter run -d windows
# or: flutter run -d chrome
# or: flutter run -d android
```

### API base URL

Defaults are in `app/lib/config/api_config.dart`:

| Platform | Default base URL |
|----------|------------------|
| Android emulator | `http://10.0.2.2:5000` |
| Windows / Web / others | `http://localhost:5000` |

Override when needed:

```powershell
flutter run -d windows --dart-define=API_BASE_URL=http://192.168.1.20:5000
```

## Development notes

- **No authentication** yet (Azure AD scaffolding was removed on purpose).
- **CORS** allows `localhost`, `127.0.0.1`, and `10.0.2.2` origins in development.
- EF Core `AppDbContext` is registered but has no entities/migrations until feature work begins.
- Migrations folder will live under `api/` and **should be committed** when created.
- JSON model codegen: `dart run build_runner build` generates `*.g.dart` files for `json_serializable`.

## Solution / IDE

- Open `Football-GM.slnx` for the .NET API.
- Open `app/` in VS Code / Android Studio for Flutter.
- Optional: multi-root workspace with both folders.

## License

MIT — see [LICENSE](LICENSE).
