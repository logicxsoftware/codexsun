You are operating in Senior Engineer Mode.

Project: cxweb (Multi-tenant ERP SaaS)

Before starting:

Read /Assist/AI.md

Enforce Tenant Isolation rules

Follow API.md contract

Follow STRUCTURE.md boundaries

Do not break existing builder logic

This task is UI + data merge refinement.
Not architecture redesign.

OBJECTIVE

Rework NavMegaMenu component to:

• Combine GLOBAL (common) menus
• Combine TENANT-SPECIFIC menus
• Display them in a single merged navigation structure
• Maintain correct order
• Preserve hierarchy

Example output:

Home | About | Services | Blog | Contact

Where:

Services

Home (common)

Services (tenant)

Features (tenant)



Rules:

• Common menus must always render
• Tenant menus must merge under correct parent
• No duplication
• No cross-tenant leakage

DATA MERGE REQUIREMENTS

You currently have:

MenuGroup
Menu (COMMON / CUSTOM)
MenuItem (nested)

Rework merge logic so that:

Fetch common menus (tenantId = null)

Fetch tenant menus (tenantId = current tenant)

Merge them at runtime

If same slug exists:

Tenant overrides common

If parent exists:

Merge children arrays

Preserve order field

No DB changes required.
Only merge logic refinement.

DISPLAY REQUIREMENTS

Top level:

Home | About | Services | Blog | Contact

Rules:

• If menu has no children → simple link
• If menu has children → dropdown
• If children > threshold → mega layout
• Maintain token-based styling
• No hardcoded colors
• Respect current theme tokens
• Respect layout alignment rules

NAVMEGAMENU LOGIC UPDATE

Refactor:

NavMegaMenu.tsx

Add:

mergeMenus(commonMenus, tenantMenus)

mergeChildren(commonChildren, tenantChildren)

deduplicate by slug

preserve order ascending

ensure stable key usage

Ensure:

• No undefined access
• No hydration mismatch
• No unnecessary re-renders
• Memoize merged result
• Use useMemo

STYLING UPDATE

Improve styling:

• Clear spacing between top items
• Hover state token-based
• Active state token-based
• Dropdown animation smooth
• Support deep nesting
• Support mobile fallback
• Respect container/full width mode
• Respect sticky behavior

No raw Tailwind colors.
Use theme tokens only.

EDGE CASES

Handle:

• No tenant-specific menus
• No common menus
• Tenant overrides entire parent
• Deep nested children
• Empty children arrays
• Inactive menu items
• Duplicate slug scenario

PERFORMANCE

• Avoid O(n²) merge logic
• Merge in single pass
• Memoize final tree
• Avoid re-render on unrelated state
• Avoid excessive DOM nodes

VALIDATION

Before finishing:

Confirm:

Combined menus render correctly

Services dropdown merges common + tenant items

No duplication

Order respected

No console errors

No cross-tenant data

No structural API change

UPDATE DISCIPLINE

If any merge logic changes affect structure:

Update:

Assist/STRUCTURE.md (if folder changed)
Assist/TENANT.md (if logic clarified)
Assist/API.md (if response shape modified)
ProjectLog.md (mandatory)

OUTPUT REQUIREMENT

Provide:

• Updated NavMegaMenu logic
• Merge utility function
• Styling adjustments
• Confirmation of Assist updates
• ProjectLog number
• Commit message format

No explanation.
No architecture redesign.
Only controlled refinement.

Proceed carefully.