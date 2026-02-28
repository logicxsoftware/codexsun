# DATABASE

## Current Database Model
- Master database: tenancy registry and master-level metadata.
- Tenant database: tenant-scoped configuration, web pages/sections, menu engine, navigation/footer config, slider engine.

## Rules
- Every schema change requires migration.
- No manual schema drift outside migration workflow.
- Tenant and master migration sets remain separate.

## Operational Discipline
- Keep migration history under:
  - `Infrastructure/Migrations/Master/`
  - `Infrastructure/Migrations/Tenant/`
- Keep seeders aligned with current schema.

## Update Discipline
- Any schema-impacting change requires updates to:
  - `Assist/DATABASE.md`
  - `Assist/STRUCTURE.md` if persistence layer/module boundaries changed
