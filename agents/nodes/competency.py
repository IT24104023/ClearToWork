import time
from agents.models.state import AgentWorkflowState, AgentStepTrace, ToolExecutionTrace
from agents.tools.api_tools import get_worker_certificates, find_eligible_workers

def competency_agent_node(state: AgentWorkflowState) -> AgentWorkflowState:
    """Student 1: Personnel & Competency Agent."""
    start_time = time.time()
    trace = AgentStepTrace(
        agent_name="Personnel & Competency Agent",
        owner="Student 1"
    )

    findings = []
    has_gap = False

    for worker_id in state.assigned_worker_ids:
        t0 = time.time()
        certs = get_worker_certificates(worker_id)
        trace.tools_called.append(ToolExecutionTrace(
            tool_name="get_worker_certificates",
            arguments={"worker_id": worker_id},
            result=certs,
            latency_ms=(time.time() - t0) * 1000
        ))

        for c in certs:
            if c.get("status") == "Expired" or c.get("daysUntilExpiry", 0) < 0:
                has_gap = True
                findings.append(f"Worker {worker_id}: Certificate {c.get('certificateName')} expired ({c.get('daysUntilExpiry')} days ago).")
            else:
                findings.append(f"Worker {worker_id}: Valid certification ({c.get('certificateName')}).")

    if has_gap:
        t0 = time.time()
        replacements = find_eligible_workers(trade="Welder", hazard_code=state.hazard_type_code)
        trace.tools_called.append(ToolExecutionTrace(
            tool_name="find_eligible_workers",
            arguments={"trade": "Welder", "hazard_code": state.hazard_type_code},
            result=replacements,
            latency_ms=(time.time() - t0) * 1000
        ))
        if replacements:
            findings.append(f"Recommended Replacement: {replacements[0].get('fullName')} ({replacements[0].get('badgeNumber')}) - {replacements[0].get('validCertificate')}.")

    state.competency_findings = findings
    trace.findings = findings
    trace.latency_ms = (time.time() - start_time) * 1000
    state.step_traces.append(trace)
    return state
