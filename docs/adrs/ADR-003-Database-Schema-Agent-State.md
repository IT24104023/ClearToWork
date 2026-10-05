# ADR-003: Relational Schema Strategy for Agent Workflow State & Audit Trails

* **Status**: ACCEPTED  
* **Date**: 2026-09-18  
* **Deciders**: ClearToWork AI Engineering Team (Zakee, Chemini, Dinithi, Oshini)  
* **Technical Area**: Relational Data Modeling & Entity Framework Core (SE3090 LO1, LO4)

---

## 1. Context & Problem Statement

In an industrial safety system, every permit lifecycle transition and automated AI clearance evaluation must be permanently recorded for post-incident audits and legal compliance (OSHA 1910.119 Process Safety Management). 

The team needed to decide how to store:
1. Core relational domain entities (PermitRequests, Workers, Assets, Zones, Approvals).
2. Transient multi-agent reasoning traces, node latencies, and synthesized proposed fixes.

---

## 2. Options Considered

* **Option A: Pure NoSQL Document Store (MongoDB)**: Allows flexible storage of nested JSON execution traces, but lacks ACID transactional guarantees, foreign key relational integrity, and standard EF Core LINQ support mandated by SE3090.
* **Option B: Dual Database Architecture (PostgreSQL for Relational + MongoDB for Agent Traces)**: Provides separation, but dramatically increases deployment complexity, operational costs, and dual-source transaction rollback failure risks.
* **Option C: Unified Relational Schema with EF Core and Structured JSON Columns**: Employs an ACID relational database where core entities have strict foreign keys and normalization, while the `AgentWorkflowRun` entity stores serialized execution traces (`ExecutionTraceJson`) and recommended remediation plans (`RecommendedFixJson`).

---

## 3. Decision

We selected **Option C: Unified Relational Schema with Entity Framework Core**:
1. Relational entities (`PermitRequests`, `Workers`, `Assets`, `Zones`, `Approvals`, `AuditEntries`) are fully modeled with strict primary/foreign keys and enum state tracking.
2. The `AgentWorkflowRun` table maintains a 1-to-1 foreign key with `PermitRequest`, capturing:
   - `OutcomeStatus` (Cleared vs RefusedSafeFailure)
   - `DurationMs` (Execution latency in milliseconds)
   - `ModelUsed` (`langgraph-5agent-pipeline`)
   - `ExecutionTraceJson` (Structured JSON array of tool calls, latencies, and findings)
   - `RecommendedFixJson` (Synthesized alternative crew badge, equipment tag, and shifted time window)
3. The EF Core context supports both PostgreSQL (`Npgsql`) for production and SQLite for zero-configuration local evaluation.

---

## 4. Consequences & Trade-offs

### Positive Consequences:
* Complete auditability without external database dependencies.
* Transactional consistency: When a permit draft is approved or rejected, its status and agent workflow run persist atomically.
* High query performance on dashboard screens via EF Core eager loading (`.Include()`).

### Mitigations:
* Implemented dedicated interactive database inspection tools (`/db` viewer and `/api/db/query/...` endpoints) to allow evaluators to inspect live stored tables in real time.
