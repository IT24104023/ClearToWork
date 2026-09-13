from langgraph.graph import StateGraph, END
from agents.models.state import AgentWorkflowState
from agents.nodes.planning import planning_agent_node
from agents.nodes.competency import competency_agent_node
from agents.nodes.equipment import equipment_agent_node
from agents.nodes.hazard import hazard_agent_node
from agents.nodes.validation import validation_agent_node

def build_permit_evaluation_graph():
    """Builds the 5-agent LangGraph workflow for ClearToWork AI."""
    workflow = StateGraph(AgentWorkflowState)

    # 1. Register distinct agent nodes
    workflow.add_node("planning", planning_agent_node)
    workflow.add_node("competency", competency_agent_node)
    workflow.add_node("equipment", equipment_agent_node)
    workflow.add_node("hazard", hazard_agent_node)
    workflow.add_node("validation", validation_agent_node)

    # 2. Wire sequential evaluation pipeline
    workflow.set_entry_point("planning")
    workflow.add_edge("planning", "competency")
    workflow.add_edge("competency", "equipment")
    workflow.add_edge("equipment", "hazard")
    workflow.add_edge("hazard", "validation")
    workflow.add_edge("validation", END)

    return workflow.compile()

compiled_graph = build_permit_evaluation_graph()
