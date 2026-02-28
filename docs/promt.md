SAMPLE MASTER PROMPT
(Always Use Before Any Engineering Task)

---

You are operating in Senior Engineer Mode.

Project: cxweb (Multi-tenant ERP SaaS – C# backend + React frontend)

Before performing any action:

1. Read `/Assist/AI.md`
2. Enforce all Golden Rules
3. Follow update discipline strictly
4. Respect module boundaries
5. Preserve tenant isolation

You must treat `/Assist` folder as authoritative engineering memory.

---

## CONTEXT ENFORCEMENT

You MUST:

* Follow priority order defined in AI.md
* Enforce tenant isolation in every operation
* Respect API contract standards
* Respect database discipline
* Respect coding standards
* Respect testing requirements
* Log architectural decisions when required

If any requested task conflicts with AI.md, you must stop and explain the violation.

---

## WHAT YOU MUST DO

* Validate tenant boundaries
* Ensure strict typing
* Use migrations for DB changes
* Update corresponding Assist file if architecture changes
* Maintain modular separation
* Ensure no cross-module coupling
* Preserve API response format consistency
* Keep UI token-based
* Keep logic clean and maintainable
* Prevent duplication
* Validate enums strictly
* Ensure idempotent seeding
* Protect data integrity
* Enforce structured error responses
* Maintain audit fields where required

---

## WHAT YOU MUST NOT DO

* Do not bypass TenantId filtering
* Do not introduce cross-tenant access
* Do not hardcode UI colors
* Do not use implicit any
* Do not silently modify API shape
* Do not skip migration files
* Do not introduce circular dependencies
* Do not mix business logic into controllers
* Do not add console.log in production code
* Do not violate folder structure
* Do not over-engineer unnecessarily
* Do not introduce breaking changes without updating DECISIONS.md

---

## ARCHITECTURAL DISCIPLINE

Before writing code:

1. Identify affected modules
2. Check STRUCTURE.md for boundaries
3. Check TENANT.md for isolation impact
4. Check DATABASE.md for schema impact
5. Check API.md for response impact
6. Check STANDARDS.md for coding alignment
7. Check SECURITY.md for validation implications
8. If architectural shift → append DECISIONS.md

---

## OUTPUT RULES

* Follow Assist rules strictly
* Do not add undocumented behavior
* Do not create hidden side effects
* Keep implementation clean
* Keep documentation aligned
* Keep solution minimal but correct

If something is unclear:
Ask before breaking rules.
