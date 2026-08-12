# Auth work handoff — resume here

**Last updated:** 2026-08-11

## Status

| PR | Title | Status |
|----|--------|--------|
| PR1 | Refresh tokens + logout + session cap/cleanup | **Done** |
| PR2 | Change password + revoke-all sessions | **Done** |
| PR3 | Auth integration tests | **Done** |
| PR4 | Production-safe config/CORS | **Done** |
| PR5 | Flutter auth client + secure storage | **Done** |
| **PR6** | Login/register UI + auth gate | **Next** |
| PR7 | Change password UI + polish | Pending |
| PR8 | Docs smoke checklist | Pending |

## PR5 summary

- `TokenStore` + `ApiClient` (Bearer + single-flight refresh) + `AuthService`
- `main()` restores session; `AuthService` / `TokenStore` provided
- No login UI yet — PR6

## Next: PR6

Login/register screens + auth gate to home when authenticated.
