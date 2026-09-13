import time
from agents.models.state import AgentWorkflowState, AgentStepTrace, ToolExecutionTrace
from agents.tools.api_tools import get_zone_conflicts, get_weather_forecast

def hazard_agent_node(state: AgentWorkflowState) -> AgentWorkflowState:
    """Student 4: Site Conditions & Hazard Control Agent."""
    start_time = time.time()
    trace = AgentStepTrace(
        agent_name="Site Conditions & Hazard Agent",
        owner="Student 4"
    )

    findings = []

    # 1. Check Zone conflicts (SIMOPS)
    t0 = time.time()
    conflicts = get_zone_conflicts(state.zone_code, state.hazard_type_code, state.start_time, state.end_time)
    trace.tools_called.append(ToolExecutionTrace(
        tool_name="get_zone_conflicts",
        arguments={
            "zone_code": state.zone_code,
            "hazard_code": state.hazard_type_code,
            "start_time": state.start_time,
            "end_time": state.end_time
        },
        result=conflicts,
        latency_ms=(time.time() - t0) * 1000
    ))

    if conflicts.get("hasConflict"):
        for c in conflicts.get("conflicts", []):
            findings.append(f"SIMOPS Clash: Conflicting Permit {c.get('conflictingPermitNumber')} in adjacent {c.get('zoneCode')} ({c.get('explanation')}).")
        if conflicts.get("suggestedAlternativeTimeWindow"):
            findings.append(f"Recommended Schedule Adjustment: {conflicts.get('suggestedAlternativeTimeWindow')}")
    else:
        findings.append("No active or adjacent zone conflicts detected.")

    # 2. Check weather limits (Open-Meteo)
    t0 = time.time()
    weather = get_weather_forecast(6.93, 79.86, state.end_time)
    trace.tools_called.append(ToolExecutionTrace(
        tool_name="get_weather_forecast",
        arguments={"lat": 6.93, "lon": 79.86, "target_time": state.end_time},
        result=weather,
        latency_ms=(time.time() - t0) * 1000
    ))

    gusts = weather.get("windGustsKmh", 0.0)
    if state.hazard_type_code == "HOT_WORK" and gusts > 35.0:
        findings.append(f"Weather Restriction: Wind gusts {gusts} km/h exceed 35 km/h limit for elevated hot work.")
    else:
        findings.append(f"Weather Clearance: Wind gusts {gusts} km/h are within safe operational parameters.")

    state.hazard_findings = findings
    trace.findings = findings
    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state
