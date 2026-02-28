# DATABASE

## Current Database Model
- Master database: tenancy registry and master-level metadata.
- Tenant database: tenant-scoped configuration, web pages/sections, menu engine, navigation/footer config, slider engine.
- `web_navigation_configs` includes `width_variant` (int, default `0`) for backend-controlled width mode.
- Home content modules (`hero`, `about`, `stats`, `catalog`, `whyChooseUs`, `brandSlider`, `features`, `callToAction`, `location`, `newsletter`) are stored as tenant-scoped section JSON payloads in existing `website_page_sections` records; no dedicated per-section tables exist in the current model.
- About page dedicated tenant tables:
  - `about_page_sections` (`tenant_id`, hero/about title+subtitle, timestamps)
  - `about_team_members` (`section_id` FK, `tenant_id`, profile fields, `display_order`)
  - `about_testimonials` (`section_id` FK, `tenant_id`, quote/rating fields, `display_order`)
  - `about_roadmap_milestones` (`section_id` FK, `tenant_id`, milestone fields, `display_order`)
- Tenant migration `20260228183305_AddAboutPageSchema` provisions About page tables with tenant/order indexes and FK constraints.
- Contact form submission table:
  - `contact_messages` (`tenant_id`, `name`, `email`, `subject`, `message`, `created_at_utc`)
  - Indexed columns: `tenant_id`, `created_at_utc`
- Tenant migration `20260228191634_AddContactMessages` provisions contact message persistence for tenant-scoped web contact submissions.

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
