import os
import uuid
import time
from fastapi import FastAPI, Header, HTTPException, status
from pydantic import BaseModel
from typing import List, Optional

from agents.models.state import AgentWorkflowState
from agents.graphs.permit_graph import compiled_graph

app = FastAPI(
    title="ClearToWork AI - Agent Orchestration Service",
    description="LangGraph Multi-Agent Clearance & Validation Engine",
    version="1.0.0"
)

SHARED_SECRET = os.getenv("AGENT_SHARED_SECRET", "ClearToWork_Internal_Agent_Key_2026")

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

@app.get("/health")
def health():
    return {"status": "healthy", "service": "cleartowork-agents", "model": "langgraph-local"}

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

    # If final_state returned as dict (LangGraph can return dict)
    if isinstance(final_state, dict):
        final_state = AgentWorkflowState(**final_state)

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

if __name__ == "__main__":
    import uvicorn
    uvicorn.run("agents.server:app", host="0.0.0.0", port=8000, reload=True)
