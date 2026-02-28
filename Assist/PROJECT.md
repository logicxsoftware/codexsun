# PROJECT

## Scope
- `cxserver` (C# backend) + `cxweb` (React frontend) + `cxtest`/Playwright tests.
- Multi-tenant ERP SaaS with tenant-scoped web content, menu/navigation, and slider systems.

## Non-Negotiable Constraints
- Security first.
- Strict tenant isolation.
- Data integrity before convenience.
- Stable API contracts for frontend consumers.
- Token-driven UI styling.

## Runtime Model
- Backend resolves tenant from request host/domain.
- Tenant-scoped data is served from tenant database context.
- Master database contains tenancy registry and onboarding metadata.

## Governance Alignment
- Structure source: `Assist/STRUCTURE.md`.
- Enforcement source: `Assist/AI.md`.
