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

## Home Data Endpoint
- `GET /api/home-data` returns tenant-resolved aggregate home payload.
- Response includes:
  - `hero` (home hero section data)
  - `about` (home about section data)
  - `stats` (home stats section data)
  - `catalog` (home catalog section data)
  - `whyChooseUs` (home why-choose-us section data)
  - `brandSlider` (home brand slider section data)
  - `features` (home features section data)
  - `callToAction` (home call-to-action section data)
  - `location` (home location section data)
  - `newsletter` (home newsletter section data)
  - `slider`
  - `navigation`
  - `footer`
  - `menus`
- `hero`, `about`, `stats`, `catalog`, `whyChooseUs`, `brandSlider`, `features`, `callToAction`, `location`, and `newsletter` are derived from published `home` page sections and include safe fallback objects when missing.
  - `callToAction` shape supports content-first fields: `title`, `description`, `buttonText`, `buttonHref` (legacy `label`/`href` accepted for backward compatibility).

## About Page Endpoint
- `GET /api/about-page` returns tenant-resolved About page composition payload.
- Response includes:
  - `hero`
  - `about`
  - `whyChooseUs`
  - `features`
  - `team`
  - `testimonials`
  - `roadmap`
  - `callToAction`
- `about`, `whyChooseUs`, `features`, and `callToAction` are sourced from published `about` page section JSON with fallback objects.
- `hero`, `team`, `testimonials`, and `roadmap` are sourced from dedicated tenant-scoped About page tables.

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
