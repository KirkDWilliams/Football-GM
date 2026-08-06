# Football GM

A more complex take on fantasy football — think franchise GM decisions, not just weekly lineups. This monorepo hosts the **ASP.NET Core API** (SQLite + JWT) and the **Flutter** client.

## Stack

| Layer | Tech |
|-------|------|
| API | ASP.NET Core 10 (`net10.0`), Entity Framework Core, SQLite, JWT bearer auth |
| Client | Flutter 3.44+ / Dart 3.12+ (Windows, Android, Web) |
| Local cache | Flutter `sqflite` (optional client scaffolding; not required for the API) |

## Repository layout

```
Football-GM/
├── api/              # ASP.NET Core Web API (FootballGm.Api)
├── app/              # Flutter client (package: football_gm_app)
├── Football-GM.slnx  # .NET solution
└── README.md
```

---

## Prerequisites

Install what you need for the parts you will run:

| Part | Requirement |
|------|-------------|
| API | [.NET 10 SDK](https://dotnet.microsoft.com/download) |
| Flutter app | [Flutter](https://docs.flutter.dev/get-started/install) (stable channel) |
| Flutter on Windows desktop | Visual Studio with the **Desktop development with C++** workload |
| Flutter on Android | Android SDK + emulator (or a physical device) |
| Flutter on Web | Chrome (or another browser Flutter supports) |
| Adding EF migrations (optional) | `dotnet tool install -g dotnet-ef` |

Check installs:

```powershell
dotnet --version
flutter doctor
```

All commands below assume your shell is at the **repo root** (`Football-GM/`) unless noted.

---

## 1. Run the API

### Start (HTTP — recommended for local Flutter)

```powershell
dotnet restore api/FootballGm.Api.csproj
dotnet run --project api/FootballGm.Api.csproj --launch-profile http
```

- Listens on **`http://localhost:5000`**
- Environment: **Development**
- On first start, creates/opens SQLite (`api/footballgm.dev.db` when the content root is `api/`) and applies migrations
- Scalar API docs open in the browser at `/scalar/v1` when the launch profile starts a browser

### Start (HTTPS profile)

```powershell
dotnet run --project api/FootballGm.Api.csproj --launch-profile https
```

- HTTPS: `https://localhost:5001`
- Also HTTP: `http://localhost:5000`
- Prefer the **http** profile when pairing with Flutter (avoids local cert friction on emulators/web)

### Useful URLs (while the API is running)

| What | URL |
|------|-----|
| Base | `http://localhost:5000` |
| Health | `http://localhost:5000/api/health` |
| Scalar UI (dev) | `http://localhost:5000/scalar/v1` |
| OpenAPI JSON | `http://localhost:5000/openapi/v1.json` |
| Mint JWT (dev only) | `POST http://localhost:5000/api/tokens` |
| Token probe | `GET http://localhost:5000/api/tokens/me` |

### Quick API checks (PowerShell)

```powershell
# Health + database
Invoke-RestMethod http://localhost:5000/api/health

# Mint a dev JWT (no user accounts yet — Development only)
$body = @{ subject = "dev-user-1"; displayName = "Dev GM" } | ConvertTo-Json
$tok = Invoke-RestMethod -Method Post -Uri http://localhost:5000/api/tokens -ContentType application/json -Body $body
$tok.accessToken

# Call a protected endpoint
Invoke-RestMethod http://localhost:5000/api/tokens/me -Headers @{ Authorization = "Bearer $($tok.accessToken)" }
```

More request samples: `api/FootballGm.Api.http`.

### Stop the API

Press `Ctrl+C` in the terminal that is running `dotnet run`.

---

## 2. Run the Flutter app

Open a **second** terminal (keep the API running if the app should call it).

### One-time / whenever dependencies change

```powershell
cd app
flutter pub get
dart run build_runner build --delete-conflicting-outputs
```

`build_runner` generates `*.g.dart` JSON helpers. Re-run it after changing models annotated with `json_serializable`.

### See available devices

```powershell
flutter devices
```

### Windows desktop

```powershell
cd app
flutter run -d windows
```

### Web (Chrome)

```powershell
cd app
flutter run -d chrome
```

### Android emulator or device

1. Start an emulator (or plug in a device with USB debugging).
2. Confirm it appears in `flutter devices`.
3. Run:

```powershell
cd app
flutter run -d android
```

The Android emulator reaches the host API at `http://10.0.2.2:5000` by default (see below).

### Override API base URL

Defaults live in `app/lib/config/api_config.dart`:

| Platform | Default API base URL |
|----------|----------------------|
| Android emulator | `http://10.0.2.2:5000` |
| Windows / Web / others | `http://localhost:5000` |

Point at another host/port:

```powershell
cd app
flutter run -d windows --dart-define=API_BASE_URL=http://192.168.1.20:5000
flutter run -d chrome --dart-define=API_BASE_URL=http://localhost:5000
flutter run -d android --dart-define=API_BASE_URL=http://10.0.2.2:5000
```

---

## 3. Run API + Flutter together

Typical local workflow:

1. **Terminal A — API**

   ```powershell
   dotnet run --project api/FootballGm.Api.csproj --launch-profile http
   ```

2. Confirm health: open `http://localhost:5000/api/health` (expect `"databaseConnected": true`).

3. **Terminal B — Flutter** (from `app/`)

   ```powershell
   cd app
   flutter pub get
   flutter run -d windows
   ```

   Use `-d chrome` or `-d android` instead if you prefer those targets.

4. Leave both processes running while you develop. Restart the API after backend changes; hot-reload Flutter for most UI changes (`r` in the Flutter terminal, or save with hot reload enabled in the IDE).

---

## 4. IDE options

| Work on | Open |
|---------|------|
| .NET API | `Football-GM.slnx` (Visual Studio / Rider) or the `api/` folder |
| Flutter app | `app/` in VS Code, Android Studio, or Cursor |
| Both | Multi-root workspace with `api/` and `app/` |

From Visual Studio you can also run the API with the **http** or **https** launch profile.

---

## SQLite (API)

- Dev file: `footballgm.dev.db` (connection string in `api/appsettings.Development.json`)
- Created/updated on startup via `Database.Migrate()`; migrations live in `api/Migrations/`
- Path is relative to the process working directory (usually `api/` for `dotnet run --project api/...`); Development logs the absolute path
- `*.db` files are gitignored — **commit migrations**, not the database
- Reset local DB: stop the API, delete `api/footballgm.dev.db` (and `-wal` / `-shm` if present), start again

---

## JWT (API — token mechanisms only)

There are **no user accounts, login screens, or password tables** yet.

| Piece | Behavior |
|-------|----------|
| Config | `Jwt` in `appsettings*.json` |
| Mint | `POST /api/tokens` — **Development only** (404 outside Development) |
| Use | `Authorization: Bearer <accessToken>` |
| Probe | `GET /api/tokens/me` |
| Protected | Teams, Players, Leagues, Games controllers |
| Anonymous | Health, token mint |

Replace `Jwt:SigningKey` before any real deployment. Prefer user secrets or environment variables for non-local secrets.

---

## Development notes

- **CORS** (dev) allows origins on `localhost`, `127.0.0.1`, and `10.0.2.2`
- **EF Core** `AppDbContext` has a baseline migration with no domain tables yet
- Flutter **does not** need its local `sqflite` DB for the API to work
- Flutter does not send JWTs yet; use Scalar, `.http`, or PowerShell to exercise auth

---

## License

MIT — see [LICENSE](LICENSE).
