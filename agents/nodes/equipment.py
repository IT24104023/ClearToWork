import time
from agents.models.state import AgentWorkflowState, AgentStepTrace, ToolExecutionTrace
from agents.tools.api_tools import check_equipment_readiness, get_isolation_points

def equipment_agent_node(state: AgentWorkflowState) -> AgentWorkflowState:
    """Student 2: Resource & Isolation Agent."""
    start_time = time.time()
    trace = AgentStepTrace(
        agent_name="Resource & Isolation Agent",
        owner="Student 2"
    )

    findings = []

    # 1. Check equipment readiness
    t0 = time.time()
    readiness = check_equipment_readiness(state.assigned_asset_tags)
    trace.tools_called.append(ToolExecutionTrace(
        tool_name="check_equipment_readiness",
        arguments={"asset_tags": state.assigned_asset_tags},
        result=readiness,
        latency_ms=(time.time() - t0) * 1000
    ))

    for res in readiness.get("results", []):
        if not res.get("isReady"):
            findings.append(f"Asset {res.get('tag')} not ready: {', '.join(res.get('reasons', []))}")
        else:
            findings.append(f"Asset {res.get('tag')}: Inspected & calibrated.")

    for repl in readiness.get("suggestedReplacements", []):
        findings.append(f"Recommended Substitute Asset: {repl.get('tag')} ({repl.get('name')}) - in-date.")

    # 2. Check isolation points for zone
    t0 = time.time()
    isolations = get_isolation_points(state.zone_id or "ZONE_B3")
    trace.tools_called.append(ToolExecutionTrace(
        tool_name="get_isolation_points",
        arguments={"zone_id": state.zone_id or "ZONE_B3"},
        result=isolations,
        latency_ms=(time.time() - t0) * 1000
    ))

    for iso in isolations:
        findings.append(f"Required Lock-Out / Tag-Out: {iso.get('tag')} ({iso.get('description')}) [{iso.get('status')}].")

    state.equipment_findings = findings
    trace.findings = findings
    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state
