import time
from agents.models.state import AgentWorkflowState, AgentStepTrace, ToolExecutionTrace
from agents.tools.api_tools import get_permit_type_template

def planning_agent_node(state: AgentWorkflowState) -> AgentWorkflowState:
    """Student 3: Planning & Coordination Agent."""
    start_time = time.time()
    trace = AgentStepTrace(
        agent_name="Planning & Coordination Agent",
        owner="Student 3"
    )

    t0 = time.time()
    template = get_permit_type_template(state.hazard_type_code)
    t_latency = (time.time() - t0) * 1000

    trace.tools_called.append(ToolExecutionTrace(
        tool_name="get_permit_type_template",
        arguments={"code": state.hazard_type_code},
        result=template,
        latency_ms=t_latency
    ))

    # Formulate structured plan (Student 3)
    state.plan_steps = [
        f"1. Safety Envelope: Enforce {template.get('name', 'Hot Work')} limits (Max {template.get('maxDurationHours', 8)}h window, Fire Watch required).",
        "2. Competency Audit: Verify worker trade qualifications and in-date certifications for all personnel.",
        "3. Equipment & LOTO: Inspect asset calibration, inspection certificates, and verify isolation lock-out points.",
        "4. SIMOPS Spatial Clearance: Evaluate 2D collision matrix and adjacent zone operational conflicts.",
        "5. Deterministic Gate: Execute multi-factor clearance validation engine before issuing permit sign-off."
    ]

    trace.findings = [
        f"Identified Hazard Class: {template.get('name', 'Hot Work')}.",
        f"Permitted Operational Envelope: Maximum {template.get('maxDurationHours', 8)} hours duration.",
        f"Safeguards Mandate: Fire watch required = {template.get('requiresFireWatch', True)}.",
        f"Structured Execution Plan: 5-Stage DAG pipeline generated ({len(state.plan_steps)} milestones)."
    ]

    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state