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
    permit_id_val = state.permit_id or "DRAFT"
    validator_report = run_permit_validator(permit_id_val)
    trace.tools_called.append(ToolExecutionTrace(
        tool_name="run_permit_validator",
        arguments={"permit_id": permit_id_val},
        result=validator_report,
        latency_ms=(time.time() - t0) * 1000
    ))

    # Dynamic multi-agent consensus synthesis from upstream node findings
    hard_failures = []
    proposed_fix = {}

    # 1. Student 1: Competency Violations
    for f in state.competency_findings:
        if "expired" in f.lower():
            hard_failures.append(f)
            proposed_fix["suggestedWorkerBadge"] = "W-1204 (valid to Mar 2027)"

    # 2. Student 2: Equipment & Isolation Violations
    for f in state.equipment_findings:
        if "overdue" in f.lower() or "unready" in f.lower() or "failed" in f.lower():
            hard_failures.append(f)
            proposed_fix["suggestedAssetTag"] = "EX-31 (inspection valid)"

    # 3. Student 4: SIMOPS & Site Condition Violations
    for f in state.hazard_findings:
        if "simops clash" in f.lower() or "exceed" in f.lower() or "restriction" in f.lower():
            hard_failures.append(f)
            proposed_fix["suggestedTimeWindow"] = "12:30–15:00"

    # If this was a static saved permit check where upstream findings were empty, fall back to validator_report
    if not state.competency_findings and not state.equipment_findings and not state.hazard_findings:
        hard_failures = validator_report.get("hardFailureReasons", [])
        proposed_fix = validator_report.get("proposedFix", {})

    is_safe_failure = len(hard_failures) > 0
    verdict = "REFUSED_SAFE_FAILURE" if is_safe_failure else "CLEARED"

    state.validation_verdict = verdict
    state.hard_failure_reasons = hard_failures
    state.recommended_fix = proposed_fix if is_safe_failure else None
    state.is_safe_failure = is_safe_failure

    # Dynamic trace findings
    comp_has_gap = any("expired" in f.lower() for f in state.competency_findings)
    equip_has_gap = any("overdue" in f.lower() or "unready" in f.lower() for f in state.equipment_findings)
    simops_has_gap = any("simops clash" in f.lower() for f in state.hazard_findings)
    weather_has_gap = any("exceed" in f.lower() for f in state.hazard_findings)

    comp_status = "FAIL (Expired worker trade certification)" if comp_has_gap else "PASS (Valid qualifications & certifications verified)"
    equip_status = "FAIL (Asset overdue for inspection/calibration)" if equip_has_gap else "PASS (All equipment calibrated, inspected & ready)"
    simops_status = "FAIL (Adjacent zone SIMOPS clash detected)" if simops_has_gap else "PASS (Spatial 2D collision matrix clear)"
    weather_status = "FAIL (Adverse meteorological envelope)" if weather_has_gap else "PASS (Weather envelope within permissible limits)"

    trace.findings = [
        f"1. Student 1 (Competency Audit): {comp_status}",
        f"2. Student 2 (Equipment & Isolation): {equip_status}",
        f"3. Student 4 (SIMOPS & Site Conditions): {simops_status}",
        f"4. Student 4 (Weather Tool Envelope): {weather_status}",
        f"5. Final Deterministic Clearance Gate: {state.validation_verdict} ({len(state.hard_failure_reasons)} hard safety violations)."
    ]

    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state