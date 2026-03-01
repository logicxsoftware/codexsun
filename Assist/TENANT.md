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
- Navigation menu rendering merges common and tenant menu trees at runtime.
- Merge precedence is tenant-over-common by slug, with ordered child merge.
- Navigation top-level ordering is primary common -> tenant-specific -> trailing common.
- Primary slugs: `home`, `about`. Trailing slugs: `blog`, `contact`.
- Render path must not duplicate slugs or leak cross-tenant menu data.
- Navigation config width fallback priority is tenant config -> global config -> `container`.
- Blog engine enforces tenant-id filtering on categories, tags, posts, post-tag joins, comments, likes, and images for all reads/writes/search operations.

## Update Discipline
- Any tenant resolution or isolation behavior change requires updates to:
  - `Assist/TENANT.md`
  - `Assist/STRUCTURE.md` if tenant-related structure boundaries changed
