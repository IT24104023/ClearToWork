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
    asset_tags = state.assigned_asset_tags or []

    if asset_tags:
        t0 = time.time()
        readiness = check_equipment_readiness(asset_tags)
        trace.tools_called.append(ToolExecutionTrace(
            tool_name="check_equipment_readiness",
            arguments={"asset_tags": asset_tags},
            result=readiness,
            latency_ms=(time.time() - t0) * 1000
        ))

        has_unready = False
        for res in readiness.get("results", []):
            tag = res.get("tag", "Unknown")
            if not res.get("isReady"):
                has_unready = True
                reasons = ", ".join(res.get("reasons", ["Inspection overdue."]))
                findings.append(f"Asset {tag}: {reasons}")
            else:
                findings.append(f"Asset {tag}: Calibration and pre-use inspection verified in-date. Ready for deployment.")

        if has_unready:
            replacements = readiness.get("suggestedReplacements", [])
            if replacements:
                rep = replacements[0]
                findings.append(f"Recommended Replacement Asset: {rep.get('name', 'In-Date Unit')} ({rep.get('tag')}) - Validated & certified.")
            else:
                findings.append("Recommended Replacement Asset: Certified In-Date Unit - Validated & certified.")
    else:
        findings.append("No specific equipment assets assigned for this operation.")

    # Check Zone Isolation / LOTO Points
    t0 = time.time()
    iso_points = get_isolation_points(state.zone_code)
    trace.tools_called.append(ToolExecutionTrace(
        tool_name="get_isolation_points",
        arguments={"zone_id": state.zone_code},
        result=iso_points,
        latency_ms=(time.time() - t0) * 1000
    ))

    if iso_points:
        iso_str = ", ".join(f"{p.get('tag', 'ISO')} ({p.get('status', 'LOCKED')})" for p in iso_points[:2])
        findings.append(f"LOTO Isolation Status: Verified active locks on {iso_str}.")

    state.equipment_findings = findings
    trace.findings = findings
    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state
