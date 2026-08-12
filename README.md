# Football GM

Fantasy football GM sim — **ASP.NET Core API** + **Flutter** app.

```
Football-GM/
├── api/       Backend (JWT auth, SQLite)
├── app/       Flutter client
├── tests/     API integration tests
└── README.md
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Flutter](https://docs.flutter.dev/get-started/install) (for the app)

## Run locally

**Terminal 1 — API** (`http://localhost:5000`):

```powershell
dotnet run --project api/FootballGm.Api.csproj --launch-profile http
```

**Terminal 2 — Flutter**:

```powershell
cd app
flutter pub get
flutter run -d windows
```

Other devices: `flutter run -d chrome` or `flutter run -d android`  
(Android emulator API URL defaults to `http://10.0.2.2:5000`.)

| Link | URL |
|------|-----|
| Health | http://localhost:5000/api/health |
| API docs (dev) | http://localhost:5000/scalar/v1 |

## Tests

```powershell
dotnet test tests/FootballGm.Api.Tests/FootballGm.Api.Tests.csproj
```

```powershell
cd app
flutter test
```

## Auth (short)

| Action | How |
|--------|-----|
| Register / login | Flutter screens or `POST /api/auth/register` · `/login` |
| Stay signed in | Access JWT + refresh token (client stores both) |
| Protected calls | `Authorization: Bearer <accessToken>` |
| Logout | App menu or `POST /api/auth/logout` |
| Change password | App menu or `POST /api/auth/change-password` |

Request samples: `api/FootballGm.Api.http`.

**Secrets:** use a real `Jwt:SigningKey` outside Development (`Jwt__SigningKey` env var). Dev keys live only in `appsettings.Development.json`.

## License

MIT — see [LICENSE](LICENSE).
