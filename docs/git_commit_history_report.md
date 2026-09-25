# ClearToWork AI — Official Git Commit History Report

**Repository**: `https://github.com/IT24104023/ClearToWork.git`  
**Generated Date**: 2026-09-23  
**Project**: SE3090 — Integrated Full-Stack and Agentic AI Application Development  

---

## 👥 Student Contribution Summary

| Student ID | Member Name | Official SLIIT Email | Primary Branch | Domain Ownership & Agent Contribution |
| :--- | :--- | :--- | :--- | :--- |
| **Student 3** | **Mohammed Zakee** | `it24104023@my.sliit.lk` | `Zakee` | Base Project Scaffolding, JWT Authentication, Permit Lifecycle State Machine, `Planning & Coordination Agent` (`planning.py`). |
| **Student 1** | **Dinithi** | `It24104198@my.sliit.lk` | `Dinithi` | Workforce & Competency Management, Trade Qualification Tracking, 30-Day Expiry Forecasting, `Personnel & Competency Agent` (`competency.py`). |
| **Student 2** | **Oshini** | `oshini@cleartowork.com` | `Oshini` | Safety Equipment Registry, 90-Day Gas Monitor Calibration Audit, LOTO Isolation Points, `Resource & Isolation Agent` (`equipment.py`). |
| **Student 4** | **Chemini** | `IT24104054@my.sliit.lk` | `Chemini` | Geospatial Site Zones, 2D SIMOPS Incompatibility Clash Matrix, Open-Meteo Weather Guardrails, `Site Conditions & Hazard Agent` (`hazard.py`). |

---

## 🌲 Git Branch & Commit Tree Topology

```
* [08f41c2] Oshini <oshini@cleartowork.com>
|           feat(equipment): implement gas monitor calibration audit, LOTO isolation points, and resource agent node
|
| * [6facf1b] Mohammed Zakee <it24104023@my.sliit.lk>
| |           feat(permits): implement Permit-to-Work core foundation, JWT authentication, and Planning Coordinator Node
|/  
| * [ee03d72] Dinithi <It24104198@my.sliit.lk>
| |           feat(workforce): implement worker trade tracking, 30-day credential expiry forecast, and competency agent node
|/  
| * [245e65f] Chemini <IT24104054@my.sliit.lk>
|/            feat(simops): implement Hazard Zones, SIMOPS clash matrix, and hazard control agent node
|
* [7cfe0e4] Mohammed Zakee <it24104023@my.sliit.lk>
            chore: remove comprehensive README for active development phase
```

---

## 📜 Complete Chronological Commit Audit Trail

### 1. Student Feature Branches (Domain Implementations)

- **`08f41c2`** | `Oshini <oshini@cleartowork.com>` | `2026-09-23`
  - `feat(equipment): implement gas monitor calibration audit, LOTO isolation points, and resource agent node`
  - *Files*: `agents/nodes/equipment.py`, `EquipmentService.cs`, `EquipmentPage.tsx`

- **`6facf1b`** | `Mohammed Zakee <it24104023@my.sliit.lk>` | `2026-09-23`
  - `feat(permits): implement Permit-to-Work core foundation, JWT authentication, and Planning Coordinator Node`
  - *Files*: `agents/nodes/planning.py`, `agents/server.py`, `AdminController.cs`

- **`ee03d72`** | `Dinithi <It24104198@my.sliit.lk>` | `2026-09-23`
  - `feat(workforce): implement worker trade tracking, 30-day credential expiry forecast, and competency agent node`
  - *Files*: `agents/nodes/competency.py`, `WorkforceDtos.cs`, `WorkforceService.cs`, `WorkforcePage.tsx`

- **`245e65f`** | `Chemini <IT24104054@my.sliit.lk>` | `2026-09-23`
  - `feat(simops): implement Hazard Zones, SIMOPS clash matrix, and hazard control agent node`
  - *Files*: `agents/nodes/hazard.py`, `HazardZoneController.cs`, `HazardDtos.cs`, `HazardRuleService.cs`, `HazardRulesPage.tsx`

---

### 2. Base Platform Foundation & Architecture (Main Branch)

- **`7cfe0e4`** | `Mohammed Zakee <it24104023@my.sliit.lk>` | `2026-09-23`
  - `chore: remove comprehensive README for active development phase`

- **`6924fd3`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-22`
  - `feat: complete CRUD for students 1-4, add Rulebook and Conflict Matrix editors, and enhance multi-agent validation trace`

- **`465ed44`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-16`
  - `feat: complete trilingual i18n, full light/dark theme parity, mobile responsiveness, and database access guides`

- **`f1d4f35`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-16`
  - `Add comprehensive root README.md covering architecture, multi-agent workflow, student ownership, and setup instructions`

- **`231f388`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-16`
  - `Fix i18n trilingual translations across all pages and enable full dynamic light/dark mode support`

- **`68575ef`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-16`
  - `Embed core engineering team photo into AboutUsPage`

- **`d34135b`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-16`
  - `Add 3D animated Landing HomePage, About Us, Contact Us, Register pages, PixelSwap, and CardNav`

- **`ece524c`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-16`
  - `Add root redirect to Swagger and /health endpoint on backend`

- **`85c0e4e`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-15`
  - `Fix Swagger RouteTemplate for reverse-proxy routing`

- **`7ea2dac`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-15`
  - `Update web/Dockerfile to node:20-alpine and configure nginx port 80/10000 with SSL proxying`

- **`387caae`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-15`
  - `Keep runtime: docker across all 3 services in render.yaml with updated nginx proxy`

- **`ad915f2`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-15`
  - `Fix render.yaml schema with type: web and runtime: static for frontend`

- **`460952b`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-15`
  - `Fix Dockerfiles, CORS, Swagger, and optimize frontend as static site in render blueprint`

- **`cb897c5`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-15`
  - `Add full-stack ClearToWork AI codebase with Docker and Render deployment setup`

- **`baa2ce1`** | `IT24104023 <mohammedzakee2006@gmail.com>` | `2026-09-13`
  - `feat: initial ClearToWork AI full-stack foundation with .NET 8 API, LangGraph agents, and React UI`

---

*Report exported automatically for academic documentation and submission.*
