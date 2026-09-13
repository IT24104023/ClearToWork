from typing import List, Optional, Dict, Any
from pydantic import BaseModel, Field

class ToolExecutionTrace(BaseModel):
    tool_name: str
    arguments: Dict[str, Any]
    result: Any
    latency_ms: float
    status: str = "SUCCESS"

class AgentStepTrace(BaseModel):
    agent_name: str
    owner: str
    tools_called: List[ToolExecutionTrace] = Field(default_factory=list)
    findings: List[str] = Field(default_factory=list)
    latency_ms: float = 0.0
    status: str = "COMPLETED"

class AgentWorkflowState(BaseModel):
    workflow_id: str
    permit_id: Optional[str] = None
    objective_description: str
    hazard_type_code: str = "HOT_WORK"
    zone_id: Optional[str] = None
    zone_code: str = "ZONE_B3"
    start_time: str
    end_time: str
    assigned_worker_ids: List[str] = Field(default_factory=list)
    assigned_asset_tags: List[str] = Field(default_factory=list)
    
    # Step results
    plan_steps: List[str] = Field(default_factory=list)
    competency_findings: List[str] = Field(default_factory=list)
    equipment_findings: List[str] = Field(default_factory=list)
    hazard_findings: List[str] = Field(default_factory=list)
    validation_verdict: str = "PENDING"
    hard_failure_reasons: List[str] = Field(default_factory=list)
    recommended_fix: Optional[Dict[str, Any]] = None
    
    # Execution Audit Trail
    step_traces: List[AgentStepTrace] = Field(default_factory=list)
    errors: List[str] = Field(default_factory=list)
    is_safe_failure: bool = False
