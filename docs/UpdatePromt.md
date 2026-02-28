UPDATE + PROJECT LOG ENFORCEMENT PROMPT
(Mandatory After Any Change)

---

You are operating in Senior Engineer Mode.

Project: cxweb (Multi-tenant ERP SaaS)

Before finalizing this task:

1. Read `/Assist/AI.md`
2. Apply Update Rules strictly
3. Update ProjectLog
4. Commit all related files with ProjectLog reference

No task is complete without memory synchronization and log entry.

---

# STEP 1 — ASSIST FILE UPDATE CHECK

If something changed, update corresponding Assist file:

Database schema changed → Update `Assist/DATABASE.md`
Tenant logic changed → Update `Assist/TENANT.md`
Folder/module structure changed → Update `Assist/STRUCTURE.md`
API contract changed → Update `Assist/API.md`
Coding conventions changed → Update `Assist/STANDARDS.md`
Security rules changed → Update `Assist/SECURITY.md`
Testing approach changed → Update `Assist/TESTING.md`
Architectural decision made → Append `Assist/DECISIONS.md`

If no update required, explicitly confirm:

"No Assist updates required."

---

# STEP 2 — PROJECT LOG UPDATE

Append new entry to:

`ProjectLog.md`

Format:

ProjectLog #: [Incremental Number]
Date: YYYY-MM-DD
Module: [Affected Module]
Type: Feature / Fix / Refactor / Migration / Test / Security
Summary: Short technical description
Files Changed: List key files
Database Impact: Yes / No
API Impact: Yes / No
Breaking Change: Yes / No

ProjectLog number must increment sequentially.

Never overwrite history.

---

# STEP 3 — COMMIT RULE

Commit message format:

`[PL-XXX] <Short Description>`

Where:

#XXX = ProjectLog number

Example:

[#042] Add slider tenant isolation validation

Rules:

* Commit all related files
* Do not split related changes across commits
* Do not commit without ProjectLog entry
* Do not commit undocumented architectural changes

---

# DO NOT

* Do not skip ProjectLog entry
* Do not modify old log numbers
* Do not commit without Assist sync
* Do not introduce silent breaking change
* Do not leave documentation outdated

---

# FINAL OUTPUT REQUIREMENT

At completion, provide:

1. ProjectLog number used
2. Assist files updated (if any)
3. Confirmation commit message format
4. Confirmation all related files included

If no structural change:

Confirm:

"No Assist update required. ProjectLog updated."

---

Memory + traceability discipline is mandatory.
