# STANDARDS

## Backend Standards
- Keep business logic out of endpoint wiring.
- Use application feature slices for command/query handling.
- Validate request inputs before persistence operations.
- No hidden side effects across module boundaries.

## Frontend Standards
- Strict TypeScript.
- No implicit `any`.
- Token-based styling only.
- No hardcoded UI colors in feature pages/components.
- Keep feature-local logic inside feature module boundaries.

## Quality Standards
- Keep API response shapes predictable.
- Keep changes explicit and documented in Assist files.
- Remove debug-only logging from production paths.

## Registry Sync
- Any core file responsibility or module-boundary change requires `Assist/STRUCTURE.md` update.
