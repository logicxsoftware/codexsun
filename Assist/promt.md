You are operating in Senior Engineer Mode.

Objective:

Perform a full system validation, optimization, and finalization pass for the codexsun platform including:

cxserver (Backend API)

cxweb (Frontend React app)

Multi-tenant routing

Dynamic section-based page engine

Performance tuning

Code clarity cleanup

Production hardening


System Context:

Clean Architecture

CQRS with MediatR

Database-per-tenant isolation

Domain-based tenant resolution

Dynamic page builder

React + TypeScript

Tailwind + shadcn

Single appsettings.json with AppEnv

Production-ready architecture

PHASE 1 – BACKEND VALIDATION

Validate all API routes:

Public web routes

Blog slug routes

Page builder endpoints

Tenant resolution middleware

Ensure:

Correct HTTP verbs

Proper status codes

Proper middleware ordering

No 500 runtime errors

No incorrect routing patterns

Proper 404 behavior

No cross-tenant leakage

Unpublished content returns 404

Tenant resolved once per request

PHASE 2 – FRONTEND ROUTE VALIDATION

Validate React Router:

Layout routing correct

Nested routes correct

Slug-based dynamic blog route working

404 fallback working

No duplicate layout wrapping

Detect and fix:

Dead routes

Confusing nesting

Circular imports

Unnecessary wrapper components

Re-render issues

Ensure:

All dynamic pages render properly

Section engine renders correctly

No console errors

Strict TypeScript passes

PHASE 3 – PERFORMANCE ANALYSIS

Backend:

Check response time:

Home page

Blog list

Blog detail

Validate:

Index usage

Query efficiency

JSON column performance

No N+1 queries

No redundant DB calls

Tenant lookup executed once

Frontend:

Measure:

Initial load time

Route transition time

Component render cost

Optimize:

Lazy loading routes where appropriate

Memoization where needed

Remove redundant state

Remove duplicate API calls

PHASE 4 – LOADING & UX TWEAKS

Implement or verify:

Proper loading states

Skeleton loading where necessary

Error boundaries

Empty states

No layout shifts

No blocking UI

Ensure:

Clean UX

No flicker

Stable transitions

PHASE 5 – CODE QUALITY & CLEANUP

Detect and fix:

Confusing abstractions

Naming inconsistencies

Misplaced files

Tight coupling

Async misuse

Missing CancellationToken

Unused services

Dead code

Over-fetching

Ensure:

Clean Architecture intact

Vertical slice clarity

No layer violations

No duplication

Strict TypeScript compliance

No console logs

No commented-out code

No unnecessary dependencies

FINAL VALIDATION BEFORE OUTPUT

Internally verify:

Backend builds successfully

Frontend builds successfully

All routes working

Tenant resolution correct

No performance regressions

No runtime errors

Production-ready stability

SaaS foundation intact

CONSTRAINTS

No comments

No markdown

No explanations

No summaries

No placeholders

Output ONLY modified or created files

Production-grade quality only

Begin full validation, cleanup, and finalization pass.