# ADR 003: Deterministic Fail-Safe Clearance Protocol

## Status
Accepted

## Context
Generative AI models are inherently non-deterministic and can hallucinate safety clearances. In high-risk environments (e.g. explosive petrochemical atmospheres), an erroneous permit approval can cause fatalities.

## Decision
ClearToWork AI enforces a **Fail-Closed by Default** policy implemented in C# and Python guardrails:
1. If any hard rule is violated (expired certificate, uncalibrated gas detector, adjacent SIMOPS clash, or wind gusts > 35 km/h), the permit evaluation is immediately terminated with verdict `REFUSED_SAFE_FAILURE`.
2. Crucially, the system does not merely reject the permit; it actively computes an **Actionable Remediation Package** (suggested valid worker, suggested calibrated asset, adjusted time window).
3. The AI agent verdict cannot bypass human sign-off: human safety officers make the final operational decision.

## Consequences
- 100% elimination of safety hallucinations.
- Guarantees fail-safe behavior even during network drops or agent timeouts.
