# AI CONTROL CENTER

This file governs all engineering memory.

Every coding session must begin with:

"Follow Assist/AI.md strictly."

This file defines:

- Golden rules
- Update discipline
- System priorities
- What to do
- What not to do
- Simplification principles

## PURPOSE

Assist folder is permanent engineering memory.

It defines structure, constraints, and discipline.

It is not optional documentation.

If Assist is outdated, system stability is compromised.

## CORE PRIORITIES (ORDER MATTERS)

1. Security
2. Tenant Isolation
3. Data Integrity
4. Predictable API Contracts
5. Performance
6. UI Consistency
7. Developer Convenience

Never reverse this order.

## GOLDEN RULES

- TenantId must exist in all tenant-bound tables.
- No cross-tenant data leakage.
- No hardcoded UI colors.
- Token-based styling only.
- Strict TypeScript on frontend.
- No implicit any.
- No console.log in production.
- Every DB change requires migration.
- Every schema change requires DATABASE.md update.
- Every architectural shift requires DECISIONS.md entry.
- No silent breaking API changes.
- No undocumented structure changes.
- No direct module-to-module tight coupling.
- Prefer modular monolith over premature microservices.
- Simplicity over complexity.

## UPDATE RULES

When something changes, update:

- Database schema -> DATABASE.md
- Tenant logic -> TENANT.md
- Folder/module structure -> STRUCTURE.md
- API response/contract -> API.md
- Coding conventions -> STANDARDS.md
- Security behavior -> SECURITY.md
- Test strategy -> TESTING.md
- Architecture decision -> DECISIONS.md

Never merge feature without updating related document.

## DO

- Keep modules isolated
- Enforce TenantId filtering
- Validate all inputs
- Keep API response consistent
- Use migrations for schema changes
- Keep UI token-driven
- Keep documentation minimal and precise
- Append decisions, never delete history

## DO NOT

- Do not bypass TenantId checks
- Do not hardcode UI colors
- Do not duplicate logic across modules
- Do not mix business logic in controllers
- Do not introduce hidden side effects
- Do not change API structure silently
- Do not skip migration files
- Do not break folder structure without updating STRUCTURE.md

## SIMPLIFICATION PRINCIPLE

When unsure:

Choose explicit over implicit.
Choose clarity over cleverness.
Choose strict typing.
Choose isolation.
Choose maintainability.

Avoid over-engineering.

## ENFORCEMENT

All engineering prompts must respect Assist folder.

All major features must align with:

- PROJECT.md
- STRUCTURE.md
- TENANT.md
- STANDARDS.md
- API.md
- DATABASE.md
- TESTING.md
- SECURITY.md
- DECISIONS.md

AI.md is the source of enforcement.