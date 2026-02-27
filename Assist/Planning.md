# Next Implementation Plan

## Phase 1: Schema and Migration Baseline
1. [x] Create and apply initial EF migration set for `MasterDbContext`.
2. [x] Create and apply initial EF migration set for `TenantDbContext`.
3. [x] Validate migration execution path in startup and onboarding flows.
4. [x] Add migration generation/run scripts for local and CI usage.

## Phase 2: Tenant Security and Secrets
1. [ ] Move database credentials out of static config into secure secret source.
2. [ ] Add encrypted-at-rest handling for tenant connection strings in master registry.
3. [ ] Add credential rotation workflow for tenant databases.
4. [ ] Add audit trail for tenant lifecycle operations.

## Phase 3: Authentication and Authorization
1. [ ] Implement JWT authentication pipeline for API.
2. [ ] Add tenant-aware authorization policies.
3. [ ] Enforce role/permission checks on onboarding and tenant admin endpoints.
4. [ ] Add integration tests for unauthorized and cross-tenant access scenarios.

## Phase 4: Operational Resilience
1. [ ] Replace in-memory distributed cache adapter with production provider.
2. [ ] Add circuit-breaker and retry policies for external dependencies.
3. [ ] Add health checks for master DB, tenant resolution, and cache connectivity.
4. [ ] Add graceful startup/degraded-mode behavior on partial infrastructure failures.

## Phase 5: Tenant Lifecycle APIs
1. [ ] Add tenant query endpoints (list, details, status).
2. [ ] Add tenant deactivation/reactivation commands.
3. [ ] Add tenant feature configuration management commands/queries.
4. [ ] Add tenant connection re-provision workflow with safety checks.

## Phase 6: Performance and Scale Hardening
1. [ ] Add cache warm-up strategy for frequently used tenant metadata.
2. [ ] Add tenant onboarding throughput controls and queue-based mode.
3. [ ] Add query-level performance profiling and slow-query telemetry.
4. [ ] Add load-test suite for multi-tenant concurrency behavior.

## Phase 7: Testing Expansion
1. [ ] Add deterministic integration test fixtures for isolated master/tenant DBs.
2. [ ] Add failure-compensation tests for onboarding rollback paths.
3. [ ] Add migration compatibility tests for MariaDB and PostgreSQL.
4. [ ] Add API contract tests for tenant endpoints and middleware behavior.

## Phase 8: Delivery and Platform Readiness
1. [ ] Add CI pipeline for restore/build/test/migration validation.
2. [ ] Add CD deployment pipeline with pre-flight checks.
3. [ ] Add Aspire environment profiles for dev/stage/prod.
4. [ ] Add runbooks for incident response, tenant recovery, and rollback.
