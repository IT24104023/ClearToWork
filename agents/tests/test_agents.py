import pytest
from fastapi.testclient import TestClient
from agents.server import app

client = TestClient(app)

def test_health_endpoint():
    response = client.get("/health")
    assert response.status_code == 200
    assert response.json()["status"] == "healthy"

def test_root_endpoint():
    response = client.get("/")
    assert response.status_code == 200
    assert response.json()["status"] == "online"

def test_agent_metrics_endpoint():
    response = client.get("/agent-metrics")
    assert response.status_code == 200
    assert "totalEvaluations" in response.json()

def test_simulate_query_overdue_asset():
    response = client.post("/simulate-query", json={
        "query": "Hot work welding in Zone B3",
        "hazard_code": "HOT_WORK",
        "zone_code": "ZONE_B3",
        "worker_id": "W-1182",
        "asset_tag": "EX-22"
    })
    assert response.status_code == 200
    data = response.json()
    assert data["verdict"] == "REFUSED_SAFE_FAILURE"
    assert any("EX-22" in f for f in data["hard_failures"])
    assert data["recommended_fix"] is not None
    assert "EX-31" in data["recommended_fix"].get("suggestedAssetTag", "")

def test_simulate_query_valid_asset_and_worker():
    response = client.post("/simulate-query", json={
        "query": "Flange bolting inspection in Zone A1",
        "hazard_code": "COLD_WORK",
        "zone_code": "ZONE_A1",
        "worker_id": "W-101",
        "asset_tag": "GAS-MON-401"
    })
    assert response.status_code == 200
    data = response.json()
    assert data["verdict"] == "CLEARED"
    assert len(data["hard_failures"]) == 0
    assert not any("EX-22" in f for f in data["hard_failures"])
