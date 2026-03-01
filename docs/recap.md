Solution Overview
Codexsun is implemented as a multi-project platform composed of codexsun.AppHost for Aspire orchestration, cxserver as a Clean Architecture backend API, and cxweb as a React frontend. The platform currently delivers tenant-isolated web experiences with dynamic CMS-driven pages, navigation/footer and slider engines, contact capture, and product catalog flows for PLP and PDP.

Architecture style
The system uses a modular monolith architecture with explicit layer boundaries. Runtime composition is API-first and tenant-context-first: requests resolve tenant identity at middleware, then execute tenant-scoped services and persistence paths. The frontend uses feature modularization with shared design-system and tokenized theming infrastructure.

Technology stack
Backend stack is .NET, ASP.NET Core minimal APIs, MediatR, FluentValidation, EF Core, MariaDB/PostgreSQL provider abstraction, and Aspire service defaults. Frontend stack is React, TypeScript strict mode, React Router, Vite, Tailwind with shadcn integration, Sonner toast, and Framer Motion for selected interactions. Testing includes backend integration tests and Playwright E2E and live suites.

Core patterns used
Core patterns include Clean Architecture layering, CQRS via request/handler slices, validation pipeline behavior, repository/store abstractions per module, domain aggregates with invariants, soft-delete with global query filters, idempotent tenant seeders, and token-driven UI governance.

Backend Architecture

Clean Architecture layers
Domain encapsulates business aggregates and invariants for tenancy, web engine, menu/navigation, slider, contact, about, product catalog, and configuration documents. Application contains abstractions, commands, queries, validators, and pipeline behaviors. Infrastructure implements persistence, tenancy resolution, migrations, caching, seeding, onboarding orchestration, and feature stores. Endpoints map HTTP contracts to application handlers and services.

CQRS implementation
CQRS is implemented through MediatR request/handler slices for commands and queries, with FluentValidation applied through a global validation pipeline behavior. Vertical slices exist across tenancy onboarding, web engine operations, menu engine operations, configuration documents, and contact submission.

Multi-tenant strategy
Tenant context is resolved per request from host/domain by middleware and resolver services. The middleware enforces invalid-host rejection, unknown tenant rejection, inactive tenant rejection, and per-request tenant context lifecycle with guaranteed cleanup after request completion.

Database-per-tenant implementation
A master database stores tenant registry metadata and tenant connection metadata. Each tenant has an isolated tenant database accessed through tenant-scoped DbContext factory/accessor. Tenant onboarding and startup initialization create, migrate, and seed tenant databases independently.

Tenant resolution logic
Resolution normalizes and validates host input, attempts direct domain lookup, and supports identifier extraction from subdomain patterns when applicable. Tenant session data includes tenant id, domain, name, and connection string; this session gates all tenant-bound operations.

Configuration refactor
Configuration is unified into a single cxserver appsettings.json with Environment and AppEnv blocks. Runtime binding resolves AppEnv:Local or AppEnv:Production sections for database, multi-tenancy cache, and auth settings. Environment-specific appsettings files were removed to avoid configuration fragmentation.

Soft delete
Soft delete is enforced through ISoftDeletable and TenantDbContext SaveChanges interception that converts deletes into soft-delete updates. Global query filters automatically exclude deleted records for tenant-bound entities.

Publish/unpublish logic
Page and section aggregates support explicit publish and unpublish operations with timestamp management. Public rendering endpoints fetch only published pages and published sections, preserving draft/inactive content isolation from public output.

Section-based CMS engine
The CMS engine persists pages and ordered sections with typed SectionType and JSON payload per section. Validation rules verify section payload shape by section type. Public endpoint GET /api/web/{slug} returns published dynamic page metadata plus ordered sections, enabling backend-driven page composition.

Frontend Architecture

Layout system (WebLayout, AppLayout, AuthLayout)
WebLayout composes tenant navigation header, content outlet, and footer via WebNavigationProvider. AppLayout provides admin/application shell with dynamic side navigation derived from menu data and fallback links. AuthLayout provides a focused token-driven authentication container using shared design primitives.

Routing structure
Routing is centralized in cxweb router with grouped layouts: web routes under root, app routes under /app, admin tools under /admin, auth under /auth, and dedicated utility routes for /theme-preview and /ui-template. Web routes include dynamic slug rendering and dedicated about, contact, products, and product detail routes.

Dynamic section rendering
Web pages are rendered by fetching backend page payloads and mapping sectionType to a typed section registry renderer. Unknown section types render a controlled unsupported-section fallback rather than failing silently.

Authentication-aware header behavior
Header auth zone behavior is state-aware through NavAuth. Unauthenticated users receive login CTA; authenticated users receive account menu actions and sign-out flow.

Global loader
A global loading store and provider wraps the app root. HTTP client calls start and stop the global loader automatically unless explicitly skipped.

Toast system
A centralized toast service and provider are integrated at app root. Feature modules emit consistent token-styled toasts for success, info, warning, and error outcomes.

Theme Engine

OKLCH usage
Theme tokens are defined with OKLCH values for both light and dark token maps, including core UI and brand/navigation/footer tokens.

CSS variable token system
Global CSS defines token-backed variables, then maps them into Tailwind theme variables. Components consume semantic tokens rather than hardcoded colors.

shadcn integration
shadcn Tailwind layer is imported and used as the component primitive foundation. Shared UI wrappers and design-system primitives standardize composition on top of these primitives.

Tenant theme injection
TenantThemeProvider loads tenant theme payload from backend, sanitizes token values, caches per host, resolves mode, and injects CSS variable overrides at runtime.

Dark/light switching
ThemeSwitch supports light, dark, and system modes with persisted preference and immediate runtime application.

Performance safeguards
Theme token application uses signature checks to avoid unnecessary DOM writes. Theme payloads are cached in memory and session storage per host. Global loader avoids duplicate ad hoc loading implementations.

UI Governance

Visual theme preview sandbox
A dedicated /theme-preview sandbox provides live token editing, validation, preset application, mode preview, and immediate visual feedback.

UI consistency audit
A governance cleanup has aligned major surfaces to token-based styling and shared primitives. Key modules such as header/footer, home sections, auth/app shells, toasts, and loaders follow consistent token-driven conventions.

Design system primitives
Reusable primitives in shared design-system provide standardized structure, typography, spacing, and card/form/layout composition patterns for feature pages.

Template page
A dedicated /ui-template page serves as canonical reference for approved patterns, component usage, states, and interaction examples.

Performance & Validation

Backend optimizations
Backend includes pooled master DbContext, tenant DbContext options caching, compiled queries in tenancy/configuration paths, retry-enabled database providers, tenant cache layers, migration batching options, and startup/onboarding hardening to migrate before seed execution.

Frontend optimizations
Frontend includes lazy loading for heavy home slider components, URL-driven product filtering/pagination, debounced search behavior, scoped rerender control with memoization in navigation and page composition, and strict typed API contracts.

Route validation
Backend query and command validators enforce slug and payload correctness. Frontend routing has explicit 404 and route error boundaries. Unknown dynamic section types degrade safely with explicit unsupported fallback UI.

Multi-tenant isolation guarantees
Tenant resolution is mandatory for tenant-bound endpoints. Tenant context is request-scoped and cleared after execution. Product/catalog/content/navigation/menu/contact reads and writes use tenant-scoped stores and tenant DbContext accessors. Soft-delete filters and published-state filters prevent hidden/draft/deleted content leakage.

Removed/Explicit Decisions

Swagger removal
Public Swagger UI exposure is not part of runtime surface. OpenAPI mapping is development-only, reducing production attack surface and unnecessary endpoint exposure.

Unified appsettings strategy
Server configuration is consolidated to one appsettings.json with Environment and AppEnv section selection. This replaced split environment files.

No environment fragmentation
Configuration, tests, and runtime selection follow one resolution model, eliminating divergent per-environment JSON file drift.

Current Platform Status

What is production-ready
Core multi-tenant backbone is implemented with domain-based tenant resolution and database-per-tenant isolation. Dynamic web rendering, section-driven CMS delivery, navigation/footer and slider engines, contact capture, about aggregate composition, and product catalog PLP/PDP flows are implemented. Theme engine, token governance, global loader, toast infrastructure, and Aspire orchestration are in place. Startup/onboarding migration and seeding reliability hardening is implemented.

What is pending
Operational hardening items remain before broad production rollout: external distributed cache and observability backends, secret management for connection/auth material, and final production deployment playbook validation in target infrastructure. CMS authoring governance for non-engineer content workflows is still backend-centric and can be expanded with dedicated admin authoring UX for page/section lifecycle.

Recommended next steps
1. Add production observability baseline with structured logs, metrics, tracing, and dashboard alerts tied to tenant resolution failures and DB connectivity.
2. Move sensitive configuration to a managed secret store and finalize environment-specific deployment variables without inline credentials.
3. Extend CMS admin surfaces for page/section authoring, preview, publish workflows, and role-based controls on top of existing backend capabilities.
4. Execute full production simulation runbook with restore drills, tenant onboarding rollback validation, and load tests across multiple tenant domains.
