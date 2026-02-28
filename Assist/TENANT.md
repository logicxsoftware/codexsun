# TENANT

## Tenant Resolution
- Tenant is resolved from request host/domain.
- Resolver path: `Infrastructure/Tenancy/TenantResolver.cs`.
- Middleware boundary: `Middleware/TenantResolutionMiddleware.cs`.

## Isolation Rules
- Tenant context must be established before tenant-bound endpoints execute.
- Tenant-bound reads/writes must go through tenant-scoped context/services.
- No cross-tenant read/write path is allowed.
- No fallback to another tenant when resolution fails.

## Data Rules
- Tenant-bound entities remain tenant-scoped.
- Seeder and migration execution must run per-tenant where applicable.

## Update Discipline
- Any tenant resolution or isolation behavior change requires updates to:
  - `Assist/TENANT.md`
  - `Assist/STRUCTURE.md` if tenant-related structure boundaries changed
