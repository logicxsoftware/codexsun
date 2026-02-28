# SECURITY

## Priority
- Security is the top system priority.

## Core Controls
- Enforce tenant isolation on every tenant-sensitive request path.
- Validate external input at API boundary.
- Reject unresolved/invalid tenant contexts for tenant-bound operations.
- Avoid exposing sensitive internals in API responses.

## Frontend Controls
- Use typed service boundaries for API calls.
- Keep token-driven styling; no unsafe dynamic style injection patterns.

## Change Discipline
- Any security behavior change requires updates to:
  - `Assist/SECURITY.md`
  - `Assist/STRUCTURE.md` if security-related boundaries changed
