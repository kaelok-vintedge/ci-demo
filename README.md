# ci-demo

A miniature of the OneActiveSpace stack whose only job is to demonstrate a
**gated CI pipeline**: every pull request runs build + tests, and the (fake)
deploys only happen when everything is green. Safe to break on purpose —
that's the point.

## What's inside

| Part | Stack | The concept it demonstrates |
|---|---|---|
| `api/Api` | .NET 10 minimal API | DTOs, validation at the boundary, service rules (capacity, double-booking, class started) |
| `api/Api.Tests` | xUnit (7 tests) | Testing business rules without a database |
| `web/` | Next.js 16 + TypeScript | `proxy.ts` auth gate, zod validation of API responses, vitest |
| `.github/workflows/ci.yml` | GitHub Actions | The quality gate: api + web jobs on every PR |
| `.github/workflows/deploy.yml` | GitHub Actions | Fake deploys that **cannot run unless tests pass** (`needs: tests`) |

## Run it locally

```bash
# terminal 1 — API on http://localhost:5252
dotnet run --project api/Api

# terminal 2 — web on http://localhost:3000
cd web && npm install && npm run dev
```

Then book a class twice, or book "Lunchtime Spin (already started)" — the
API's rules reject both, and `api/Api.Tests` proves those rules stay working.

## Run the checks (same as CI)

```bash
dotnet test api/Api.Tests
cd web && npm run lint && npm run typecheck && npm run test && npm run build
```

## The pipeline

- **Pull request** → `ci.yml` runs the api and web jobs. Red = don't merge.
- **Merge to `develop`** → `deploy.yml` runs the same tests, then the fake
  staging deploy — only if tests pass.
- **Production** → manual "Run workflow" on `main`; the click is the
  approval, the tests are still the gate.

To use this on a real project: copy both workflows, replace the echo steps
in `deploy.yml` with real deploy steps (see the real repo's docker/ECS
steps), done.
