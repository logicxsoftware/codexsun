# API

## Contract Shape
- Endpoint groups currently include:
  - Home
  - Web content
  - Menu management
  - Web navigation/footer
  - Slider
  - Configuration documents
  - Tenant context
  - Tenants onboarding/context

## Contract Rules
- No silent breaking contract changes.
- Status codes and payload structure must remain stable per endpoint.
- Tenant-sensitive endpoints must enforce tenant context.
- Navigation config responses expose `widthVariant` as lowercase string (`container|full|boxed`), not numeric enum.

## Change Discipline
- Any contract change requires updates to:
  - `Assist/API.md`
  - `Assist/STRUCTURE.md` if API surface/module boundaries changed
  - Frontend consumers in related feature services
