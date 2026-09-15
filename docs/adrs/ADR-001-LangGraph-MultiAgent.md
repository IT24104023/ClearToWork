# ADR 001: Adoption of LangGraph for Multi-Agent Orchestration

## Status
Accepted

## Context
The SE3090 Assignment 1 specification requires an identifiable, multi-agent workflow where each group member owns a distinct agent with its own responsibility, input/output schema, and tool allow-list. Single-prompt monolithic LLM calls or generic chatbots without deterministic boundaries do not qualify.

## Decision
We adopted **LangGraph** (Python) to orchestrate a 5-node sequential Directed Acyclic Graph (DAG) over a strongly-typed `AgentWorkflowState`:
1. Node 1: Planning & Coordination Agent (Student 3)
2. Node 2: Personnel & Competency Agent (Student 1)
3. Node 3: Resource & Isolation Agent (Student 2)
4. Node 4: Site Conditions & Hazard Control Agent (Student 4)
5. Node 5: Validation & Safety Agent (Shared Engine)

## Consequences
### Positive
- Strict separation of duties per student with individual tool allow-lists.
- Full deterministic auditability: each step produces an `AgentStepTrace` with millisecond latency and tool findings.
- Clear state transitions between nodes.

### Negative
- Requires inter-process communication between ASP.NET Core and the Python FastAPI runtime, managed via an internal HTTP client and shared cryptographic secret.
