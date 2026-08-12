# Flutter client (`app/`)

```
lib/
├── main.dart              # start app, wire dependencies
├── app.dart               # MaterialApp + providers
├── config/                # API base URL
├── auth/                  # login, tokens, auth screens
├── core/network/          # Dio client + general API
├── features/home/         # home UI + teams list state
├── data/                  # local DB + repositories
└── models/                # shared domain models
```

## Run

Start the API first, then:

```powershell
flutter pub get
flutter run -d windows
```

Override API URL if needed:

```powershell
flutter run -d windows --dart-define=API_BASE_URL=http://192.168.1.10:5000
```

See the [root README](../README.md) for the full monorepo setup.
