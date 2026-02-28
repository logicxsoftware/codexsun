# Project Log

## 2026-02-27

- Created `Assist/AGENT.md`.
- Created `Assist/ProjectLog.md`.
- Implemented foundational Clean Architecture structure inside `cxserver` with `Domain`, `Application`, `Infrastructure`, and API integration.
- Added domain base abstractions and aggregates for soft delete, entity identity, tenant registry (`Tenant`), and tenant configuration document storage.
- Added application abstractions for tenant context, tenant resolver, tenant registry, tenant migration service, current user, date/time, configuration document store, and unit of work.
- Added vertical-slice application handlers and validators for configuration document query and upsert flows.
- Implemented master/tenant database-per-tenant infrastructure with `MasterDbContext`, `TenantDbContext`, dynamic tenant DbContext factory/accessor, and provider-agnostic EF Core configuration for MariaDB/PostgreSQL.
- Implemented global soft delete query filters and request-scoped tenant connection resolution pipeline.
- Added tenant resolution middleware with request-scoped tenant session and tenant connection binding.
- Added startup database initialization flow with master migration, tenant migration, default tenant seeding, and default tenant configuration seeding.
- Added design-time DbContext factories for separate master and tenant migration sets.
- Updated `cxserver` configuration structure for provider selection, master DB settings, tenant connection template, and multi-tenancy options.
- Fixed `cxtest` broken project reference by replacing missing `cxsun` reference with `cxserver`.
- Verified successful builds for `cxserver` and `cxtest`.

---
# 2: 2026-02-27T15:40:39+05:30
Version: #1.0.0
- Implemented performance optimizations for multi-tenant architecture including metadata/feature caching abstractions, compiled queries, pooled master DbContext, tenant DbContext options caching, migration batching, and provider-aware connection pooling configuration.
- Implemented automated tenant onboarding vertical slice with CQRS command, FluentValidation, handler orchestration, idempotent duplicate handling, retry-safe infrastructure executors, compensation cleanup, activation flow, and onboarding API endpoint.
- Integrated onboarding infrastructure services for tenant database creation/deletion, migration execution, seeding execution, feature configuration initialization, and coordinator wiring.
- Updated multi-tenant configuration options and appsettings for cache TTL, migration batching/parallelism, server version, and connection pool sizing.
- Verified `cxserver` build success after onboarding and optimization changes.

---
# 3: 2026-02-27T15:58:21+05:30
Version: #1.0.0
- Updated `cxserver` MariaDB configuration to use dev master database `codexsun_dev` and default tenant database `tenant_dev` with root credentials.
- Added tenancy behavior integration tests in `cxtest` covering startup seeding validation and tenant onboarding idempotency/database provisioning.
- Added database reset logic in tests for deterministic execution against local MariaDB.
- Added no-migration fallback (`EnsureCreatedAsync`) in startup and onboarding migration paths for master and tenant databases.
- Executed and passed tenancy test suite: `dotnet test cxtest/cxtest.csproj` (2/2 passed).
- Verified extra `src` projects were unrelated to current `cxserver` architecture.
- Removed unused solution projects and folders: `src/Codexsun.Domain`, `src/Codexsun.Application`, `src/Codexsun.Infrastructure`.
- Cleaned solution and confirmed successful full build: `dotnet build codexsun.sln` with zero errors.

---
# 5: 2026-02-27T16:08:10+05:30
Version: #1.0.0
- Added browser home endpoint for `cxserver` at `/` rendering a welcome page with service startup timestamp.
- Registered runtime startup timestamp singleton and mapped home endpoint in API startup pipeline.
- Updated tenant resolution middleware bypass paths to allow home page and infrastructure endpoints without tenant header.
- Verified `cxserver` builds successfully after home page and middleware updates.

---
# 6: 2026-02-27T17:08:26+05:30
Version: #1.0.0
- Established cxweb production-ready frontend foundation with feature-based structure (features, layouts, 
outes, shared).
- Implemented layout architecture with WebLayout, AppLayout, and AuthLayout using TailwindCSS and shadcn components.
- Implemented centralized route configuration with grouped nested routes for /, /app, /auth, plus typed route error and 404 handling.
- Added shared environment/config and centralized HTTP client for API-first readiness without hardcoded backend URLs.
- Replaced starter UI shell with router-based app entry and strict root bootstrap handling.
- Stabilized frontend configuration: fixed TypeScript aliasing, added vite-env.d.ts, aligned Vite alias resolution, and removed incompatible router plugin wiring.
- Fixed strict/lint blockers in UI components and configuration; validated clean 
pm run lint and successful 
pm run build.


---
# 7: 2026-02-27T18:42:20+05:30
Version: #1.0.0
- Added root deployment artifacts for containerized Aspire AppHost: Dockerfile, .dockerignore, and root docker-compose.yml on codexion-network with MariaDB expected as an external/separate container.
- Removed MariaDB service from .devcontainer/docker-compose.yml to keep DB lifecycle separate from devcontainer/runtime orchestration.
- Fixed AppHost container startup behavior by disabling launch profiles in container run (--no-launch-profile) and binding host endpoints through compose env vars.
- Implemented static port mapping across Aspire and project settings to remove dynamic ports: AppHost 7040, cxserver HTTP 7041, cxserver HTTPS 7042, frontend 7043.
- Updated codexsun.AppHost/AppHost.cs endpoint configuration to pin frontend and use external server endpoints without endpoint duplication conflicts.
- Updated codexsun.AppHost/Properties/launchSettings.json, cxserver/Properties/launchSettings.json, and cxweb/vite.config.ts to align static local/dev ports.
- Updated root docker-compose.yml published ports to 7040-7043 plus Aspire support ports 7045-7047.
- Verified container runtime and connectivity: AppHost listens on http://0.0.0.0:7040; host TCP checks passed for 7040, 7041, 7042, and 7043.


---
# 8: 2026-02-27T19:12:48+05:30
Version: #1.0.0
- Added dedicated production container build file Dockerfile.prod for Ubuntu/server deployment.
- Added dedicated production compose file docker-compose.prod.yml with static ports (7040, 7041, 7042, 7043, 7045, 7046, 7047) and 
estart: unless-stopped.
- Added deployment documentation Assist/Server-installation.md with step-by-step Ubuntu setup: Docker install, repository setup, production appsettings usage, external MariaDB setup on codexion-network, deploy, verification, and update flow.
- Validated production compose configuration using docker compose -f docker-compose.prod.yml config.


---
# 14: 2026-02-27T15:40:39
Version: #1.0.0
- Standardized configuration management to a single appsettings.json using Environment and AppEnv blocks for Local and Production.
- Removed environment-specific configuration files and updated runtime binding to resolve settings from AppEnv:{Environment}.
- Updated test configuration loading to use unified environment-resolved settings and validated tenant behavior with local MariaDB credentials.

---
# 15: 2026-02-27T22:42:10+05:30
Version: #1.0.0
- Implemented domain-based tenant resolution from request host with normalization, secure host validation, inactive/unknown tenant handling, and strict per-request tenant context isolation.
- Added tenant domain lookup abstractions and infrastructure implementation, persisted tenant domain in master tenancy model, and introduced master migration for tenant domain column/index.
- Removed header-based tenant connection accessor flow and switched tenant DbContext resolution to scoped tenant context connection string to prevent cross-request leakage.
- Added tenant context query slice and API endpoint for tenant-scoped response retrieval without exposing connection strings.
- Refactored configuration to a single appsettings.json using Environment + AppEnv blocks and updated runtime/test binding to environment-scoped configuration sections.
- Removed appsettings.Development.json and appsettings.Production.json and updated deployment documentation to unified configuration structure.
- Re-ran tenancy tests against localhost/root/DbPass1@@ with codexsun_db and tenant1_db; fixed migration discovery issue and achieved passing test suite (2/2).

---
# 16: 2026-02-27T23:42:08.3533278+05:30
Version: #1.0.0
- Implemented dynamic section-based website engine with tenant-scoped page aggregate, section entity, publish/unpublish controls, section ordering invariants, and JSON section payload support.
- Added WebEngine application slices for page and section lifecycle operations, published page retrieval by slug, validation rules, and section data validation strategy.
- Added tenant infrastructure store and EF configurations for website pages/sections with publish, slug, and ordering indexes and tenant-db-only content access.
- Updated public web API to a single tenant-resolved dynamic endpoint GET /api/web/{slug} returning published page metadata and ordered published sections.
- Refactored frontend web routing to dynamic slug rendering and implemented section registry renderer with strongly typed section payload contracts and unknown-section fallback.
- Removed legacy hardcoded web page components and obsolete intermediate web-content path, then validated backend build, frontend build, and tenancy tests successfully.

---
# 17: 2026-02-27T23:53:14.4845834+05:30
Version: #1.0.0
- Added tenant website content seeder to create and publish default dynamic pages (home, bout, services, log, contact) with ordered section payloads.
- Wired website content seeding into both startup tenant initialization and onboarding tenant seeding flows for consistent tenant bootstrap behavior.
- Updated local default tenant domain to localhost so domain-based tenant resolution works correctly for local web requests.
- Fixed frontend web API path construction for dynamic page fetch to avoid duplicate /api prefix and resolve /api/web/{slug} correctly.
- Revalidated backend build and tenancy tests after seeding and routing fixes.

---
# 18: 2026-02-28T00:57:07+05:30
Version: #1.0.0
- Implemented full cxweb OKLCH tokenized tenant theme engine with runtime tenant theme loading, validation, sanitization, scoped CSS variable application, dark/light/system mode handling, and cached per-tenant theme resolution.
- Refactored web layouts and major UI surfaces (header, footer, cards, sections, toast, loader, links, auth/app shells) to token-first styling and theme-aware behavior.
- Added development-only visual theme preview sandbox at /theme-preview with real-time token editor, OKLCH validation, debounced live updates, preset themes, light/dark preview switching, and isolated scoped rendering.
- Added reusable design-system primitive layer (PageContainer, SectionContainer, ContentWrapper, Title, Subtitle, BodyText, Divider, CardWrapper, ButtonWrapper, FormGroup, SectionHeader, GridLayout) and adopted it across key pages.
- Added canonical UI baseline reference page at /ui-template demonstrating approved layout patterns, typography scale, component variants, forms, alerts, sidebar/navigation samples, toast/loader triggers, CTA and footer patterns.
- Completed frontend governance cleanup pass and validated successful frontend build and lint after all changes.
- http://localhost:7043/theme-preview
---
# 19: 2026-02-28T08:16:33.7437952+05:30
Version: #1.0.0
- Implemented full tenant-aware menu management foundation with domain models, application abstractions, infrastructure store, API endpoints, and tenant-db schema for menu groups, menus, and hierarchical menu items.
- Added web navigation and footer configuration resources with tenant/global resolution, admin/public API endpoints, EF persistence mapping, and seeding integration.
- Implemented cxweb dynamic navigation architecture with provider-based config loading, token-driven header/footer render components, and admin builder route /admin/web-menu-builder.
- Added Codexsun-specific default seeding for Header, Footer, Mobile, and Sidemenu menu groups with complete SaaS menu trees, mega/dropdown structures, and mobile mirrored hierarchy.
- Added Codexsun footer ecosystem defaults including about section, legal links, social links, business hours, newsletter enablement, payment providers, and bottom bar developed-by configuration.
- Updated default tenant settings to codexsun with localhost domain and tenant1_db target database, and ensured onboarding seeding executes website, menu, and navigation seeders idempotently.
- Verified backend and frontend builds after changes.

