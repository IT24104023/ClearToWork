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

    trace.findings = [
        f"Validation Verdict: {state.validation_verdict}",
        f"Total Hard Failures: {len(state.hard_failure_reasons)}"
    ]

    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state
