# Auth work handoff — resume here

**Last updated:** 2026-08-11

## Status

| PR | Title | Status |
|----|--------|--------|
| PR1 | Refresh tokens + logout + session cap/cleanup | **Done** |
| PR2 | Change password + revoke-all sessions | **Done** |
| PR3 | Auth integration tests | **Done** |
| **PR4** | Production-safe config/CORS | **Next** (or PR5 Flutter) |
| PR4 | Production-safe config/CORS | Pending |
| PR5–7 | Flutter auth | Pending |
| PR8 | Docs smoke checklist | Pending |

## PR2 summary

- `POST /api/auth/change-password` (Bearer)
- Body: `currentPassword`, `newPassword` (min 8, must differ)
- Success: rehash + revoke **all** refresh sessions → 204
- Client must login again

## PR3 shipped

- Project: `api.Tests/FootballGm.Api.Tests`
- `dotnet test api.Tests/FootballGm.Api.Tests.csproj` — 9 auth/health tests
- Testing env uses `EnsureCreated` (migration chain not empty-DB safe)

## Next: PR4 or PR5

- PR4: production secrets / CORS
- PR5: Flutter auth client + secure storage
