# ClearToWork AI — Multi-Agent Workflow (LangGraph)

## 1. Design Overview
The agentic subsystem is implemented using **LangGraph** in Python. Instead of relying on a single monolithic prompt, ClearToWork AI executes a sequential Directed Acyclic Graph (DAG) with five specialized agent nodes operating over a strongly-typed shared state model (`AgentWorkflowState`).

```
[Start]
   │
   ▼
[Node 1: Planning & Coordination Agent (Student 3)]
   │
   ▼
[Node 2: Personnel & Competency Agent (Student 1)]
   │
   ▼
[Node 3: Resource & Isolation Agent (Student 2)]
   │
   ▼
[Node 4: Site Conditions & Hazard Agent (Student 4)]
   │
   ▼
[Node 5: Validation & Safety Agent (Shared Engine)]
   │
   ▼
[Verdict: CLEAR or REFUSED_SAFE_FAILURE]
```

---

## 2. Shared State Model (`AgentWorkflowState`)

```python
class AgentWorkflowState(BaseModel):
    workflow_id: str
    permit_id: Optional[str]
    objective_description: str
    hazard_type_code: str
    zone_code: str
    start_time: str
    end_time: str
    assigned_worker_ids: List[str]
    assigned_asset_tags: List[str]

    # Cumulative Findings
    plan_steps: List[str]
    competency_findings: List[str]
    equipment_findings: List[str]
    hazard_findings: List[str]
    validation_verdict: str
    hard_failure_reasons: List[str]
    recommended_fix: Optional[Dict[str, Any]]

    # Observability & Audit Traces
    step_traces: List[AgentStepTrace]
    is_safe_failure: bool
```

---

## 3. Tool Execution Boundaries & Allow-Lists

Each agent is strictly restricted to its allowed tools:

### Agent 1: Planning & Coordination
- **Allowed Tools:** `get_permit_type_template(code: str)`
- **Behavior:** Fetches hazard duration limits, requires fire watch flag, and builds structured 5-step safety verification plan.

### Agent 2: Personnel & Competency
- **Allowed Tools:** `get_worker_certificates(worker_id: str)`, `find_eligible_workers(trade: str, hazard_code: str)`
- **Behavior:** Queries certificate expiries. If expired, automatically retrieves qualified replacement workers.

### Agent 3: Resource & Isolation
- **Allowed Tools:** `check_equipment_readiness(asset_tags: List[str])`, `get_isolation_points(zone_id: str)`
- **Behavior:** Validates inspection date and calibration status. Recommends verified replacement assets if overdue.

### Agent 4: Site Conditions & Hazard Control
- **Allowed Tools:** `get_zone_conflicts(zone_code, hazard_code, start, end)`, `get_weather_forecast(lat, lon, target_time)`
- **Behavior:** Queries adjacent zone permits for SIMOPS clashes. Enforces 35 km/h wind gust ceiling from Open-Meteo.

### Agent 5: Validation & Safety Guardrail
- **Allowed Tools:** `run_permit_validator(permit_id: str)`
- **Behavior:** Enforces fail-safe clearance. If any hard violation exists, marks `is_safe_failure = True`, outputs `REFUSED_SAFE_FAILURE`, and synthesizes the remediation package.
