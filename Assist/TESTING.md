# TESTING

## Backend Tests
- Project: `cxtest`
- Coverage focus:
  - Tenancy behavior
  - Integration scenarios (migration, seeding, menu, navigation, slider, footer)
  - Backend e2e scenarios

## Frontend Tests
- Project: `cxweb/tests`
- Coverage focus:
  - Playwright e2e flows
  - Live server/API transport scenarios
  - Tenant isolation and console/network health checks

## Validation Rules
- Tenant isolation checks are mandatory for tenant-sensitive changes.
- API contract-impacting changes require corresponding test updates.
- Migration-impacting changes require migration/integration verification.
