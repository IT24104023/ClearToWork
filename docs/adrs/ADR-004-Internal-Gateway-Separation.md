# ADR-004: Internal Gateway Separation: ASP.NET Core API as Sole Client Interface

* **Status**: ACCEPTED  
* **Date**: 2026-09-20  
* **Deciders**: ClearToWork AI Engineering Team (Zakee, Chemini, Dinithi, Oshini)  
* **Technical Area**: Security Architecture & Service Boundary (SE3090 Mandatory Rule)

---

## 1. Context & Problem Statement

SE3090 Assignment 1 specifies a mandatory architectural constraint:
> *"Mandatory backend rule: React and Flutter must communicate only with the ASP.NET Core Web API. Where a Python Agentic AI service is used, it must operate as an internal service called by ASP.NET Core and must not be called directly by either client application."*

The architecture required strict physical boundary isolation between client applications (Web and Mobile) and the internal LangGraph Python microservice.

---

## 2. Options Considered

* **Option A: Public Agent Endpoints with Client-Side Token Passing**: Allow React to call the Python agent directly via CORS. Direct violation of the assignment specification; exposes internal prompt logic and agent tool execution directly to public browsers.
* **Option B: ASP.NET Core as Reverse Proxy & Orchestration Gateway**: All client interactions (Login, Permit Creation, Workflow Submission) terminate at the ASP.NET Core API. When AI review is requested (`POST /api/Permits/{id}/submit`), the ASP.NET Core service acts as a secure upstream gateway, invoking the Python agent over an internal network with cryptographic header verification (`X-Agent-Secret`).

---

## 3. Decision

We implemented **Option B: ASP.NET Core as the Authoritative Gateway**:
1. Neither the React Web application nor the Flutter Mobile application contains references or HTTP calls to the Python agent service.
2. The `PermitLifecycleService` in ASP.NET Core constructs a standardized domain payload and invokes `http://cleartowork-agent:8000/evaluate-permit` using `IHttpClientFactory`.
3. Mutual authentication is enforced using a shared secret header (`X-Agent-Secret: ClearToWork_Internal_Agent_Key_2026`).
4. ASP.NET Core validates and unpacks the returned verdict, updates the database entity status, and returns a sanitized DTO (`ValidationReportDto`) to the client.

---

## 4. Consequences & Trade-offs

### Positive Consequences:
* 100% compliant with the SE3090 mandatory architectural constraint.
* Centralized authentication: All role-based access control (RBAC) claims are verified in ASP.NET Core before computing expensive agent workflows.
* Resilience: If the Python agent experiences cold-start delays, ASP.NET Core provides deterministic fallback handling without crashing client apps.
