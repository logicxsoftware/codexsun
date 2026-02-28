# STRUCTURE

## Scope
Current-state structure only. No planned modules are listed.

## Repository Roots
- `Assist/` : Engineering memory and governance files.
- `codexsun.AppHost/` : Aspire orchestration host.
- `cxserver/` : Backend API and application runtime.
- `cxweb/` : Frontend React application.
- `cxtest/` : Backend automated tests.
- `docs/` : Project documentation and `ProjectLog.md`.
- `scripts/` : Migration scripts.

## Backend (`cxserver`)

### Entry and API Surface
- `Program.cs` : Startup pipeline, middleware, endpoint mapping.
- `Endpoints/` : API endpoint groups.
  - `HomeEndpoints.cs`
  - `WebContentEndpoints.cs`
  - `MenuManagementEndpoints.cs`
  - `WebNavigationEndpoints.cs`
  - `SliderEndpoints.cs`
  - `ConfigurationDocumentsEndpoints.cs`
  - `TenantContextEndpoints.cs`
  - `TenantsEndpoints.cs`
- `Middleware/TenantResolutionMiddleware.cs` : Request tenant resolution boundary.

### Application Layer
- `Application/Abstractions/` : Ports/contracts.
- `Application/Behaviors/` : Cross-cutting request behaviors.
- `Application/DependencyInjection/` : Application layer registrations.
- `Application/Features/` : Vertical slices.
  - `Contact/`
  - `ConfigurationDocuments/`
  - `MenuEngine/`
  - `Tenants/`
  - `WebEngine/`

### Domain Layer
- `Domain/Common/`
- `Domain/AboutPage/`
- `Domain/ContactEngine/`
- `Domain/Configuration/`
- `Domain/MenuEngine/`
- `Domain/NavigationEngine/`
- `Domain/SliderEngine/`
- `Domain/Tenancy/`
- `Domain/WebEngine/`

### Infrastructure Layer
- `Infrastructure/DependencyInjection/` : Infrastructure registrations.
- `Infrastructure/Persistence/` : EF DbContexts, configurations, converters, design-time factories.
- `Infrastructure/Migrations/` : Master and tenant migrations plus initialization hosted service.
- `Infrastructure/Tenancy/` : Tenant resolver, registry, domain lookup, context accessor, connection builder.
- `Infrastructure/Seeding/` : Master and tenant seeders.
- `Infrastructure/WebEngine/` : Web content persistence services.
- `Infrastructure/MenuEngine/` : Menu persistence services.
- `Infrastructure/NavigationEngine/` : Navigation/footer persistence services.
- `Infrastructure/ContactEngine/` : Contact message persistence services.
- `Infrastructure/SliderEngine/` : Slider persistence services.
- `Infrastructure/ConfigurationStorage/` : Configuration document storage/unit of work.
- `Infrastructure/Onboarding/` : Tenant onboarding flow executors.
- `Infrastructure/Caching/` : Tenant metadata and feature caches.
- `Infrastructure/Options/` : Bound settings objects.
- `Infrastructure/Identity/` : Current user implementation.
- `Infrastructure/Time/` : Date/time provider.

### Backend Module Boundaries
- Endpoint layer calls Application layer (MediatR-style slices).
- Application depends on Domain + Abstractions, not concrete Infrastructure implementations.
- Infrastructure implements Abstractions and owns DB, migration, seeding, tenancy resolution details.

## Frontend (`cxweb`)

### App Entry and Routing
- `src/main.tsx` : Bootstrap.
- `src/App.tsx` : Root composition.
- `src/routes/router.tsx` : Route map.
- `src/layouts/` : `WebLayout`, `AppLayout`, `AuthLayout`.

### Feature Modules
- `src/features/app/`
- `src/features/auth/`
- `src/features/menu-admin/`
- `src/features/slider/`
- `src/features/theme-preview/`
- `src/features/ui-template/`
- `src/features/web/`
- `src/features/web-navigation/`

### About Page Composition (`src/features/web/pages/about`)
- Dedicated frontend route: `/about` (mapped in `src/routes/router.tsx`) uses an About-specific page composition instead of slug-rendered generic sections.
- Blocks:
  - `blocks/TeamSection.tsx`
  - `blocks/TestimonialsSection.tsx`
  - `blocks/RoadmapSection.tsx`
- API consumer:
  - `src/features/web/services/about-page-api.ts`

### Contact Page Composition (`src/features/web/pages/contact`)
- Dedicated frontend routes: `/contact` and `/web-contacts` (mapped in `src/routes/router.tsx`) render a dynamic contact experience instead of slug-rendered generic sections.
- API consumer:
  - `src/features/web/services/contact-page-api.ts`

### Home Section Composition (`src/features/web`)
- Home page section rendering remains driven by backend `web/{slug}` section payload order.
- Home dynamic section components currently include:
  - `AboutSection.tsx`
  - `StatsSection.tsx`
  - `CatalogSection.tsx`
  - `WhyChooseUsSection.tsx`
  - `BrandSliderSection.tsx`
  - `FeaturesSection.tsx`
  - `CallToActionSection.tsx`
  - `LocationSection.tsx`
  - `NewsletterSection.tsx`
- Supporting UI primitives for these sections include:
  - `Counter.tsx`
  - `CatalogCard.tsx`
  - `components/ui/marquee.tsx`

### Shared and UI Layers
- `src/shared/auth/`
- `src/shared/config/`
- `src/shared/services/`
- `src/shared/theme/`
- `src/shared/components/`
- `src/components/ui/` : Reusable UI primitives.
- `src/lib/` : Shared utility helpers.
- `src/css/` : Global styles.

### Frontend Module Boundaries
- Feature modules own pages/components/services/types/utils for their domain.
- `shared/` contains cross-feature services, theme system, and shared UI wrappers.
- `components/ui/` provides reusable primitives consumed by features and shared components.

## Builder Systems

### Slider Builder System
- Frontend: `src/features/slider/pages/SliderBuilderPage.tsx`, `context/`, `services/`, `components/`, `types/`, `utils/`.
- Backend: `Endpoints/SliderEndpoints.cs`, `Domain/SliderEngine/`, `Infrastructure/SliderEngine/`, tenant migration and seeding support.

### Menu Builder System
- Frontend: `src/features/menu-admin/pages/`, `components/`, `services/`, `types/`, `utils/`.
- Backend: `Endpoints/MenuManagementEndpoints.cs`, `Application/Features/MenuEngine/`, `Domain/MenuEngine/`, `Infrastructure/MenuEngine/`.

### Navigation and Footer Builder System
- Frontend: `src/features/web-navigation/pages/WebMenuBuilderPage.tsx`, `services/navigation-api.ts`, `components/Nav*.tsx`, `components/Footer*.tsx`.
- Backend: `Endpoints/WebNavigationEndpoints.cs`, `Domain/NavigationEngine/`, `Infrastructure/NavigationEngine/`.

## Shared Layers Across Backend and Frontend
- Tenant context and isolation are enforced in backend tenancy middleware/services and consumed by frontend through tenant-scoped API behavior.
- Web rendering uses backend web/navigation/slider modules and frontend `web`, `web-navigation`, and `slider` features.
- Home aggregate data endpoint `/api/home-data` supplements section-driven rendering by exposing tenant-resolved `hero`, `about`, `stats`, `catalog`, `whyChooseUs`, `brandSlider`, `features`, `callToAction`, `location`, and `newsletter` blocks plus navigation/footer/slider/menu context.

## Tests
- `cxtest/` : Backend test project.
  - Root tests: tenancy behavior assembly setup.
  - `integration/` : Integration tests.
  - `e2e/` : Backend E2E flows.
  - `TestKit/` : Shared backend test environment utilities.
- `cxweb/tests/` : Frontend Playwright and live test suites.
  - `e2e/`
  - `live/`
  - `fixtures/`
  - `utils/`

## Config and Build/Run Systems
- Backend config: `cxserver/appsettings.json`, `cxserver/Properties/launchSettings.json`.
- Frontend config: `cxweb/package.json`, `cxweb/tsconfig*.json`, `cxweb/vite.config.ts`, `cxweb/playwright*.ts`, `cxweb/eslint.config.js`.
- AppHost config: `codexsun.AppHost/AppHost.cs`, `codexsun.AppHost/appsettings*.json`, `codexsun.AppHost/Properties/launchSettings.json`.
- Migration scripts: `scripts/migrations-local.ps1`, `scripts/migrations-ci.ps1`.

## Generated/Runtime Artifacts (Non-Source)
- Backend: `bin/`, `obj/`.
- Frontend: `node_modules/`, `dist/`, `playwright-report/`, `playwright-report-live/`, `test-results/`, `obj/`.
- Tests: `bin/`, `obj/`.
