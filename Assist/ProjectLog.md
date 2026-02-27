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
Timestamp: 2026-02-27T15:40:39+05:30
Level: INFO
- Implemented performance optimizations for multi-tenant architecture including metadata/feature caching abstractions, compiled queries, pooled master DbContext, tenant DbContext options caching, migration batching, and provider-aware connection pooling configuration.
- Implemented automated tenant onboarding vertical slice with CQRS command, FluentValidation, handler orchestration, idempotent duplicate handling, retry-safe infrastructure executors, compensation cleanup, activation flow, and onboarding API endpoint.
- Integrated onboarding infrastructure services for tenant database creation/deletion, migration execution, seeding execution, feature configuration initialization, and coordinator wiring.
- Updated multi-tenant configuration options and appsettings for cache TTL, migration batching/parallelism, server version, and connection pool sizing.
- Verified `cxserver` build success after onboarding and optimization changes.

---
Timestamp: 2026-02-27T15:58:21+05:30
Level: INFO
- Updated `cxserver` MariaDB configuration to use dev master database `codexsun_dev` and default tenant database `tenant_dev` with root credentials.
- Added tenancy behavior integration tests in `cxtest` covering startup seeding validation and tenant onboarding idempotency/database provisioning.
- Added database reset logic in tests for deterministic execution against local MariaDB.
- Added no-migration fallback (`EnsureCreatedAsync`) in startup and onboarding migration paths for master and tenant databases.
- Executed and passed tenancy test suite: `dotnet test cxtest/cxtest.csproj` (2/2 passed).
- Verified extra `src` projects were unrelated to current `cxserver` architecture.
- Removed unused solution projects and folders: `src/Codexsun.Domain`, `src/Codexsun.Application`, `src/Codexsun.Infrastructure`.
- Cleaned solution and confirmed successful full build: `dotnet build codexsun.sln` with zero errors.

---
Timestamp: 2026-02-27T16:08:10+05:30
Level: INFO
- Added browser home endpoint for `cxserver` at `/` rendering a welcome page with service startup timestamp.
- Registered runtime startup timestamp singleton and mapped home endpoint in API startup pipeline.
- Updated tenant resolution middleware bypass paths to allow home page and infrastructure endpoints without tenant header.
- Verified `cxserver` builds successfully after home page and middleware updates.
