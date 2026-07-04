# ci-demo

Miniature class-booking app (mirrors the OneActiveSpace stack) that exists to
demonstrate a gated CI pipeline. It is a sandbox: breaking it is allowed,
bypassing its pipeline is not — the pipeline is the product here.

## Tech stack
- `api/` — .NET 10 minimal API, in-memory store (no database on purpose)
- `web/` — Next.js 16 (App Router) + TypeScript + zod; use `proxy.ts`, NOT `middleware.ts`
- Tests: xUnit (`api/Api.Tests`), vitest (`web/src/**/*.test.ts`)

## Commands
```bash
dotnet run --project api/Api          # API on :5252
dotnet test api/Api.Tests             # backend tests
cd web && npm run dev                 # web on :3000
cd web && npm run lint && npm run typecheck && npm run test && npm run build
```

## Rules
- Never push to `main` or `develop` directly — PRs only; the human merges.
- All CI checks must be green before a PR is merged. Never weaken or skip a
  failing check to get green — fix the code, or flag the conflict.
- Keep it miniature: no database, no new dependencies, no real deploys.
  If a change needs infrastructure, it belongs in a real repo, not here.
- New booking rules in `BookingService` ship with a test in `Api.Tests`.

## Pipeline map
- `ci.yml` — quality gate (api + web jobs), runs on every PR and via
  `workflow_call` from deploy.yml.
- `deploy.yml` — fake deploys gated by `needs: tests`; staging auto on
  `develop`, production via manual `workflow_dispatch` on `main`.
