import time
from agents.models.state import AgentWorkflowState, AgentStepTrace, ToolExecutionTrace
from agents.tools.api_tools import run_permit_validator

def validation_agent_node(state: AgentWorkflowState) -> AgentWorkflowState:
    """Shared: Validation & Safety Agent."""
    start_time = time.time()
    trace = AgentStepTrace(
        agent_name="Validation & Safety Agent",
        owner="Shared"
    )

    t0 = time.time()
    validator_report = run_permit_validator(state.permit_id or "DRAFT")
    trace.tools_called.append(ToolExecutionTrace(
        tool_name="run_permit_validator",
        arguments={"permit_id": state.permit_id or "DRAFT"},
        result=validator_report,
        latency_ms=(time.time() - t0) * 1000
    ))

    # Deterministic validation output
    state.validation_verdict = validator_report.get("verdict", "REFUSED_SAFE_FAILURE")
    state.hard_failure_reasons = validator_report.get("hardFailureReasons", [])
    state.recommended_fix = validator_report.get("proposedFix")
    state.is_safe_failure = not validator_report.get("isApproved", False)

    # Detailed domain validation traces
    competency_status = "FAIL (Expired certification)" if any("Welder" in r or "Worker" in r or "Certificate" in r for r in state.hard_failure_reasons) else "PASS (Valid certifications)"
    equipment_status = "FAIL (Overdue inspection/calibration)" if any("Asset" in r or "Equipment" in r or "inspection" in r for r in state.hard_failure_reasons) else "PASS (Equipment ready & certified)"
    simops_status = "FAIL (Adjacent zone hazard collision)" if any("SIMOPS" in r or "Zone" in r or "clash" in r or "solvent" in r.lower() for r in state.hard_failure_reasons) else "PASS (No spatial conflicts)"
    weather_status = "FAIL (Adverse weather)" if any("Wind" in r or "Weather" in r for r in state.hard_failure_reasons) else "PASS (Weather envelope verified)"

    trace.findings = [
        f"1. Student 1 (Competency Audit): {competency_status}",
        f"2. Student 2 (Equipment & Isolation): {equipment_status}",
        f"3. Student 4 (SIMOPS & Site Conditions): {simops_status}",
        f"4. Student 4 (Weather Tool Envelope): {weather_status}",
        f"5. Final Deterministic Clearance Gate: {state.validation_verdict} ({len(state.hard_failure_reasons)} hard safety violations)."
    ]

    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state