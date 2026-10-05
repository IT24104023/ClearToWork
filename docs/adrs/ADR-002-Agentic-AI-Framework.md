# ADR-002: LangGraph Directed Acyclic Graph (DAG) Multi-Agent Orchestration Framework

* **Status**: ACCEPTED  
* **Date**: 2026-09-15  
* **Deciders**: ClearToWork AI Engineering Team (Zakee, Chemini, Dinithi, Oshini)  
* **Technical Area**: Agentic AI Orchestration & Verification (SE3090 LO2, LO4)

---

## 1. Context & Problem Statement

High-hazard Permit-to-Work clearance involves multiple concurrent safety domains:
1. Workforce competency & certification validity (OPITO, CompEx).
2. Safety asset calibration intervals & LOTO isolation states.
3. Concurrent operational hazards (SIMOPS clashes) and live weather envelope constraints.
4. Deterministic final clearance verification under a strict **Fail-Closed Safety Policy**.

Using a single monolithic LLM prompt or unconstrained autonomous agents introduces non-deterministic hallucinations, infinite planning loops, and unpredictable response latencies unacceptable in safety-critical oil & gas environments. The team required a deterministic multi-agent orchestration framework supporting structured DAG pipelines, inspectable execution traces, and safe failure remediation.

---

## 2. Options Considered

* **Option A: AutoGen / CrewAI**: Emphasizes open-ended conversational agent loops. While flexible for creative tasks, conversational emergence is inherently non-deterministic, difficult to audit, and prone to unbounded token latency in production.
* **Option B: LangGraph (StateGraph DAG)**: Models agent interactions as a formal Directed Acyclic Graph (DAG). Provides fine-grained state schemas, deterministic branching conditions, persistent step-by-step checkpoints, and sub-second tool execution.
* **Option C: Custom Procedural Scripting (Pure Python/C#)**: Deterministic, but lacks standardized agent state abstractions, tool-calling interfaces, and modular node extensibility required by the SE3090 agentic curriculum.

---

## 3. Decision

We selected **LangGraph** with a **StateGraph Directed Acyclic Graph (DAG)** orchestration pipeline implemented in Python with FastAPI:
1. **Planning & Coordination Node** (Student 3 - Zakee): Validates duration envelopes, fire watch mandates, and initializes the DAG state.
2. **Personnel & Competency Node** (Student 1 - Dinithi): Executes worker credential validation against trade requirement matrices.
3. **Resource & Isolation Node** (Student 2 - Oshini): Checks 90-day bump test and calibration validity and LOTO point locks.
4. **Site Conditions & Hazard Node** (Student 4 - Chemini): Evaluates adjacent SIMOPS zone clashes and queries the live **Open-Meteo Weather API**.
5. **Validation & Guardrail Node** (Shared): Evaluates cross-agent findings. If any violation is present, safely returns `REFUSED_SAFE_FAILURE` with a synthesized proposed fix (alternate worker, alternate asset, or rescheduled window).

---

## 4. Consequences & Trade-offs

### Positive Consequences:
* **Fail-Closed Safety Guarantee**: Zero hallucinated permits; permits are rejected by default unless all 5 nodes pass.
* **Auditability**: Every agent execution produces structured JSON traces with sub-millisecond latencies recorded in `AgentWorkflowRun`.
* **Clear Student Ownership**: Each student owns and evaluates an identifiable node in the LangGraph pipeline.

### Mitigations:
* FastAPI wrapper exposes `/evaluate-permit` and `/agent-metrics` behind an internal shared secret (`X-Agent-Secret`), preventing direct external access.
