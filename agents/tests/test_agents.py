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
