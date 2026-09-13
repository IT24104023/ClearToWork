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

    # Formulate structured plan
    state.plan_steps = [
        f"1. Verify {template.get('name', 'Hazard')} operational safety envelope.",
        "2. Audit assigned worker competencies and trade certifications.",
        "3. Validate equipment readiness, calibration, and isolation points.",
        "4. Evaluate spatial-temporal SIMOPS conflicts with adjacent plant zones.",
        "5. Execute deterministic clearance rule validation before human sign-off."
    ]

    trace.findings = [
        f"Identified hazard type: {template.get('name')}.",
        f"Maximum allowed window: {template.get('maxDurationHours', 8)} hours.",
        f"Fire watch required: {template.get('requiresFireWatch', True)}."
    ]

    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state
