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

## Contact Page Endpoints
- `GET /api/contact-page` returns tenant-resolved contact page composition payload.
- Response includes:
  - `hero` (contact hero section JSON from published `contact` page with fallback)
  - `location` (contact location section JSON from published `contact` page, then `home` fallback, then safe default)
- `POST /api/contact` accepts tenant-scoped contact submissions.
- Request body:
  - `name` (required)
  - `email` (required, valid email)
  - `subject` (optional)
  - `message` (required)
- Response:
  - `201 Created` with `{ id, createdAtUtc }` for the stored message.
- Contact submission validation is enforced in the application layer and persisted per tenant.

## Product Catalog Endpoint
- `GET /api/products` returns tenant-scoped PLP data with backend filtering, sorting, and pagination.
- Query parameters:
  - `category` (slug)
  - `minPrice`, `maxPrice`
  - `search`
  - `sort` (`latest|price_asc|price_desc|name_asc|name_desc`)
  - `page`, `pageSize`
  - dynamic attributes via `attr_<key>=<value>` (repeatable query params)
- Response shape:
  - `data` (paginated product list projection)
  - `pagination` (`page`, `pageSize`, `totalItems`, `totalPages`, `hasPrevious`, `hasNext`)
  - `filters` (`categories`, dynamic `attributes`, `priceRange`)
- Tenant enforcement:
  - Product queries are always scoped by backend tenant context (`tenant_id`) and `is_active`.
- `GET /api/products/{slug}` returns tenant-scoped product detail payload for PDP.
- PDP response includes:
  - `product` object:
    - `id`, `name`, `slug`
    - `description`, `shortDescription`
    - `price`, `comparePrice`
    - `inStock`
    - `categoryName`, `categorySlug`
    - `images` (ordered image URLs)
    - `specifications` (key/value dictionary)
    - `sku`
  - `relatedProducts` (same-category tenant-scoped subset, excludes current product, newest first, limited)

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
