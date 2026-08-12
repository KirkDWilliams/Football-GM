# Auth work handoff — resume here

**Last updated:** 2026-08-11

## Status

| PR | Title | Status |
|----|--------|--------|
| PR1 | Refresh tokens + logout + session cap/cleanup | **Done** |
| PR2 | Change password + revoke-all sessions | **Done** |
| PR3 | Auth integration tests | **Done** |
| PR4 | Production-safe config/CORS | **Done** |
| **PR5** | Flutter auth client + secure storage | **Next** |
| PR6 | Login/register UI + auth gate | Pending |
| PR7 | Change password UI + polish | Pending |
| PR8 | Docs smoke checklist | Pending |

## PR4 summary

- Non-Development rejects placeholder `Jwt:SigningKey` at startup
- CORS: local hosts only in Development/Testing; prod uses `Cors:AllowedOrigins`
- User secrets id on API project; README secrets section
- Env vars: `Jwt__SigningKey`, `Cors__AllowedOrigins__0`, etc.

## Next: PR5

Flutter: token storage, Dio Bearer interceptor, refresh-on-401, auth API methods.
