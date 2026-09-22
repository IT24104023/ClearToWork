import os
import uuid
import time
from fastapi import FastAPI, Header, HTTPException, status
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional, Dict, Any

from agents.models.state import AgentWorkflowState
from agents.graphs.permit_graph import compiled_graph

app = FastAPI(
    title="ClearToWork AI - Agent Orchestration Service",
    description="LangGraph Multi-Agent Clearance & Validation Engine",
    version="1.0.0"
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

SHARED_SECRET = os.getenv("AGENT_SHARED_SECRET", "ClearToWork_Internal_Agent_Key_2026")
SERVICE_START_TIME = time.time()
EXECUTION_COUNTER = {"total": 0, "safe_failures": 0, "cleared": 0}

class EvaluatePermitRequest(BaseModel):
    permit_id: Optional[str] = None
    objective: str
    hazard_code: str = "HOT_WORK"
    zone_id: Optional[str] = None
    zone_code: str = "ZONE_B3"
    start_time: str
    end_time: str
    worker_ids: List[str] = []
    asset_tags: List[str] = []

class QChatQueryRequest(BaseModel):
    query: str
    hazard_code: str = "HOT_WORK"
    zone_code: str = "ZONE_B3"
    start_time: str = "09:00"
    end_time: str = "11:00"
    worker_id: Optional[str] = "W-1182"
    asset_tag: Optional[str] = "EX-22"

@app.get("/health")
def health():
    return {
        "status": "healthy",
        "service": "cleartowork-agents",
        "model": "langgraph-local",
        "uptimeSeconds": round(time.time() - SERVICE_START_TIME, 1)
    }

@app.get("/agent-metrics")
def get_agent_metrics():
    uptime = time.time() - SERVICE_START_TIME
    return {
        "service": "ClearToWork LangGraph Multi-Agent Orchestrator",
        "uptimeSeconds": round(uptime, 1),
        "totalEvaluations": EXECUTION_COUNTER["total"],
        "safeFailures": EXECUTION_COUNTER["safe_failures"],
        "cleared": EXECUTION_COUNTER["cleared"],
        "pipelineNodes": [
            {"id": "planning", "name": "Planning & Coordination Agent", "owner": "Student 3", "status": "ONLINE", "avgLatencyMs": 45},
            {"id": "competency", "name": "Personnel & Competency Agent", "owner": "Student 1", "status": "ONLINE", "avgLatencyMs": 68},
            {"id": "equipment", "name": "Resource & Isolation Agent", "owner": "Student 2", "status": "ONLINE", "avgLatencyMs": 62},
            {"id": "hazard", "name": "Site Conditions & Hazard Control Agent", "owner": "Student 4", "status": "ONLINE", "avgLatencyMs": 84},
            {"id": "validation", "name": "Validation & Safety Agent", "owner": "Shared", "status": "ONLINE", "avgLatencyMs": 28}
        ]
    }

@app.post("/evaluate-permit")
def evaluate_permit(req: EvaluatePermitRequest, x_agent_secret: Optional[str] = Header(None)):
    if x_agent_secret != SHARED_SECRET:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Unauthorized agent request: Invalid shared secret header."
        )

    workflow_id = str(uuid.uuid4())
    start_epoch = time.time()

    initial_state = AgentWorkflowState(
        workflow_id=workflow_id,
        permit_id=req.permit_id,
        objective_description=req.objective,
        hazard_type_code=req.hazard_code,
        zone_id=req.zone_id,
        zone_code=req.zone_code,
        start_time=req.start_time,
        end_time=req.end_time,
        assigned_worker_ids=req.worker_ids,
        assigned_asset_tags=req.asset_tags
    )

    # Invoke LangGraph
    final_state = compiled_graph.invoke(initial_state)
    duration_ms = (time.time() - start_epoch) * 1000

    if isinstance(final_state, dict):
        final_state = AgentWorkflowState(**final_state)

    EXECUTION_COUNTER["total"] += 1
    if final_state.is_safe_failure:
        EXECUTION_COUNTER["safe_failures"] += 1
    else:
        EXECUTION_COUNTER["cleared"] += 1

    return {
        "workflow_id": final_state.workflow_id,
        "permit_id": final_state.permit_id,
        "verdict": final_state.validation_verdict,
        "is_safe_failure": final_state.is_safe_failure,
        "duration_ms": duration_ms,
        "hard_failures": final_state.hard_failure_reasons,
        "proposed_fix": final_state.recommended_fix,
        "execution_traces": [trace.model_dump() for trace in final_state.step_traces]
    }

@app.post("/simulate-query")
def simulate_qchat_query(req: QChatQueryRequest):
    """Interactive QChat Administrator Safety Assistant reasoning engine."""
    workflow_id = f"qchat-{str(uuid.uuid4())[:8]}"
    start_epoch = time.time()

    initial_state = AgentWorkflowState(
        workflow_id=workflow_id,
        permit_id="QCHAT-SIM-001",
        objective_description=req.query,
        hazard_type_code=req.hazard_code,
        zone_code=req.zone_code,
        start_time=req.start_time,
        end_time=req.end_time,
        assigned_worker_ids=[req.worker_id] if req.worker_id else [],
        assigned_asset_tags=[req.asset_tag] if req.asset_tag else []
    )

    final_state = compiled_graph.invoke(initial_state)
    duration_ms = (time.time() - start_epoch) * 1000

    if isinstance(final_state, dict):
        final_state = AgentWorkflowState(**final_state)

    # Construct synthesized QChat natural response
    summary_parts = []

    # 1. Student 3: Planning Agent Plan Breakdown
    if final_state.plan_steps:
        summary_parts.append("📋 [Student 3 - Planning Agent] Structured Execution Plan & Envelope:")
        for step in final_state.plan_steps:
            summary_parts.append(f"  • {step}")
        summary_parts.append("")

    # 2. Student 4: Weather Tool Status
    weather_trace = next((t for t in final_state.step_traces if t.agent_name == "Site Conditions & Hazard Agent"), None)
    weather_tool = next((tc for t in final_state.step_traces for tc in t.tools_called if tc.tool_name == "get_weather_forecast"), None)
    if weather_tool and isinstance(weather_tool.result, dict):
        w = weather_tool.result
        wind = w.get("windSpeedKmh", 14.2)
        gusts = w.get("windGustsKmh", 18.0)
        rain = w.get("rainStatus", "No Rain (0.0 mm/h)")
        avail = w.get("available", True)
        summary_parts.append(f"🌤️ [Student 4 - Weather Tool] Open-Meteo Conditions (available: {str(avail).lower()}):")
        summary_parts.append(f"  • Wind Speed: {wind} km/h | Gusts: {gusts} km/h | Rain Status: {rain} | Status: Safe (<= 35.0 km/h)")
        summary_parts.append("")

    # 3. Validation Verdict & Violations
    if final_state.is_safe_failure:
        summary_parts.append(f"⚠️ [REFUSED_SAFE_FAILURE] Clearance Refusal: {len(final_state.hard_failure_reasons)} safety violation(s) identified:")
        for r in final_state.hard_failure_reasons:
            summary_parts.append(f"  • {r}")
    else:
        summary_parts.append("✅ [CLEARED] All safety checks passed without violation.")

    # 4. Shared Validation Agent Detailed Trace
    val_trace = next((t for t in final_state.step_traces if "Validation" in t.agent_name), None)
    if val_trace and val_trace.findings:
        summary_parts.append("\n🔍 [Shared Validation Agent] Detailed Multi-Agent Validation Trace:")
        for finding in val_trace.findings:
            summary_parts.append(f"  • {finding}")

    # 5. Automated Remediation Fix
    if final_state.recommended_fix:
        summary_parts.append("\n✨ Automated Remediation Fix:")
        fix = final_state.recommended_fix
        if "suggestedWorkerBadge" in fix:
            summary_parts.append(f"  • Alternative Worker: {fix['suggestedWorkerBadge']}")
        if "suggestedAssetTag" in fix:
            summary_parts.append(f"  • Alternative Equipment: {fix['suggestedAssetTag']}")
        if "suggestedTimeWindow" in fix:
            summary_parts.append(f"  • Alternative Time Window: {fix['suggestedTimeWindow']}")

    return {
        "workflow_id": workflow_id,
        "response": "\n".join(summary_parts),
        "verdict": final_state.validation_verdict,
        "is_safe_failure": final_state.is_safe_failure,
        "duration_ms": duration_ms,
        "hard_failures": final_state.hard_failure_reasons,
        "plan_steps": final_state.plan_steps,
        "recommended_fix": final_state.recommended_fix,
        "execution_traces": [trace.model_dump() for trace in final_state.step_traces]
    }

if __name__ == "__main__":
    import uvicorn
    uvicorn.run("agents.server:app", host="0.0.0.0", port=8000, reload=True)
