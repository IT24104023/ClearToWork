from agents.models.state import AgentWorkflowState
from agents.nodes.planning import planning_agent_node
from agents.nodes.competency import competency_agent_node
from agents.nodes.equipment import equipment_agent_node
from agents.nodes.hazard import hazard_agent_node
from agents.nodes.validation import validation_agent_node

try:
    from langgraph.graph import StateGraph, END

    workflow = StateGraph(AgentWorkflowState)

    workflow.add_node("planning", planning_agent_node)
    workflow.add_node("competency", competency_agent_node)
    workflow.add_node("equipment", equipment_agent_node)
    workflow.add_node("hazard", hazard_agent_node)
    workflow.add_node("validation", validation_agent_node)

    workflow.set_entry_point("planning")
    workflow.add_edge("planning", "competency")
    workflow.add_edge("competency", "equipment")
    workflow.add_edge("equipment", "hazard")
    workflow.add_edge("hazard", "validation")
    workflow.add_edge("validation", END)

    compiled_graph = workflow.compile()
except Exception:
    class SequentialPermitGraph:
        def invoke(self, state: AgentWorkflowState) -> AgentWorkflowState:
            state = planning_agent_node(state)
            state = competency_agent_node(state)
            state = equipment_agent_node(state)
            state = hazard_agent_node(state)
            state = validation_agent_node(state)
            return state

    compiled_graph = SequentialPermitGraph()
