import pytest
from agents.models.state import AgentWorkflowState
from agents.graphs.permit_graph import build_permit_evaluation_graph
from agents.tools.api_tools import (
    get_permit_type_template,
    get_worker_certificates,
    check_equipment_readiness,
    get_zone_conflicts,
    get_weather_forecast
)

def test_tool_get_permit_type_template():
    template = get_permit_type_template("HOT_WORK")
    assert template["code"] == "HOT_WORK"
    assert template["maxDurationHours"] == 8
    assert template["requiresFireWatch"] is True

def test_tool_get_worker_certificates_identifies_expired():
    certs = get_worker_certificates("worker-1182")
    assert len(certs) == 1
    assert certs[0]["status"] == "Expired"
    assert certs[0]["daysUntilExpiry"] < 0

def test_tool_check_equipment_readiness_identifies_overdue():
    res = check_equipment_readiness(["EX-22", "GAS-DET-01"])
    assert res["allReady"] is False
    assert any(not r["isReady"] and "EX-22" in r["tag"] for r in res["results"])
    assert len(res["suggestedReplacements"]) > 0

def test_tool_get_zone_conflicts():
    conflicts = get_zone_conflicts("ZONE_B3", "HOT_WORK", "09:00", "11:00")
    assert conflicts["hasConflict"] is True
    assert len(conflicts["conflicts"]) == 1
    assert conflicts["conflicts"][0]["ruleCode"] == "HR-07"

def test_langgraph_full_workflow_execution():
    graph = build_permit_evaluation_graph()
    initial_state = AgentWorkflowState(
        workflow_id="wf-test-001",
        permit_id="PTW-TEST-001",
        objective_description="Hot work cutting steel bracket on Zone B3 mezzanine",
        hazard_type_code="HOT_WORK",
        zone_code="ZONE_B3",
        start_time="09:00",
        end_time="11:00",
        assigned_worker_ids=["worker-1182"],
        assigned_asset_tags=["EX-22"]
    )

    final = graph.invoke(initial_state)
    if isinstance(final, dict):
        final = AgentWorkflowState(**final)

    # Verify all 5 agents executed in sequence
    assert len(final.step_traces) == 5
    agent_names = [t.agent_name for t in final.step_traces]
    assert "Planning & Coordination Agent" in agent_names
    assert "Personnel & Competency Agent" in agent_names
    assert "Resource & Isolation Agent" in agent_names
    assert "Site Conditions & Hazard Agent" in agent_names
    assert "Validation & Safety Agent" in agent_names

    # Check safe failure outcome
    assert final.is_safe_failure is True
    assert final.validation_verdict == "REFUSED_SAFE_FAILURE"
    assert len(final.hard_failure_reasons) > 0
    assert final.recommended_fix is not None
