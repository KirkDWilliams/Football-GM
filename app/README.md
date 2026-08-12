# Football GM — Flutter client

Mobile/desktop/web client for Football GM.

See the [root README](../README.md) for full monorepo setup, API URL notes, and how to run both sides.

## Auth client (PR5)

Wiring for the ASP.NET auth API lives under `lib/services/`:

| Type | Role |
|------|------|
| `TokenStore` | Persist access + refresh session via `flutter_secure_storage` |
| `ApiClient` | Shared Dio: attach `Authorization: Bearer`, refresh on 401 |
| `AuthService` | `register`, `login`, `refresh`, `logout`, `me`, `changePassword` |
| `ApiService` | Domain calls using the same Dio instance |

On startup, `main()` loads any stored session so protected API calls can send a JWT. Login/register **UI** is not built yet (PR6).

```dart
// Example (no UI):
await authService.login(email: 'gm@example.com', password: 'correct-horse-battery');
final me = await authService.me();
await authService.logout();
```
