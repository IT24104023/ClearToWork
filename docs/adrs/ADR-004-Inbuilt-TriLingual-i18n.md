# ADR 004: Inbuilt Tri-Lingual Localization (English, Sinhala, Tamil)

## Status
Accepted

## Context
Industrial field workers, contractors, and supervisors in Sri Lanka operate across English, Sinhala (සිංහල), and Tamil (தமிழ்). Relying on external translation APIs (e.g. Google Translate) introduces external API costs, privacy leakage of industrial records, latency, and potential network failure in low-connectivity plant areas.

## Decision
We implemented a native, **inbuilt dictionary-based i18n localization engine** embedded directly within the client bundle (`web/src/i18n/translations.ts` and `web/src/context/I18nContext.tsx`).

## Consequences
- Zero external network requests or subscription costs for localization.
- Instant, zero-latency language switching.
- 100% offline capability suitable for remote industrial facilities.
- Standardized, consistent domain terminology for safety hazards, permits, and agent states across all three languages.
