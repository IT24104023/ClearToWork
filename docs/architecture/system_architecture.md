# ClearToWork AI — System Architecture Specification

## 1. Executive Summary & Problem Domain
ClearToWork AI is an enterprise industrial **Permit-to-Work (PTW)** and **Safety Clearance Engine** designed for high-risk petrochemical processing plants, refineries, and fabrication yards. In these high-hazard facilities, catastrophic incidents (fires, toxic releases, explosions) routinely occur due to simultaneous incompatible activities (SIMOPS), uncalibrated safety equipment, or unauthorized personnel.

ClearToWork AI replaces fragmented paper-based and manual spreadsheet systems with an autonomous, multi-agent evaluation pipeline that deterministically verifies safety envelopes prior to human sign-off.

---

## 2. 4-Tier Integrated Architecture

ClearToWork AI is organized into four distinct architectural layers:

```
┌─────────────────────────────────────────────────────────────┐
│                   Tier 1: Presentation                      │
│   • React 19 + Vite Web Client (Tailwind CSS, Redux Toolkit) │
│   • Native Trilingual Localization: English, Sinhala, Tamil │
│   • Dark / Light Industrial UI with Live QChat Terminal      │
└──────────────────────────────┬──────────────────────────────┘
                               │ HTTPS / JSON REST API
┌──────────────────────────────▼──────────────────────────────┐
│                Tier 2: Backend Application                  │
│   • ASP.NET Core 8 Web API Gateway                          │
│   • JWT Bearer Authentication & Cryptographic RBAC Matrix   │
│   • Deterministic Safety Rule Engine & Exception Handlers   │
│   • Third-Party Open-Meteo Weather Client with Caching      │
└──────────────────────────────┬──────────────────────────────┘
                               │
            ┌──────────────────┴──────────────────┐
            │ EF Core Relational                  │ Internal HTTP + Secret
┌───────────▼─────────────┐           ┌───────────▼─────────────┐
│   Tier 3: Relational    │           │    Tier 4: Agentic AI   │
│   Data Persistence      │           │    Orchestrator         │
│ • EF Core 8 Provider    │           │ • Python FastAPI Service│
│ • Normalized Schema     │           │ • LangGraph StateGraph  │
│ • Transactional LOTO    │           │ • 5 Autonomous Agents   │
│ • Full Audit Logging    │           │ • Fail-Closed Consensus │
└─────────────────────────┘           └─────────────────────────┘
```

---

## 3. Student Component Ownership (Marking Scheme Alignment)

| Student | Functional Component | Agentic AI Contribution | Primary Tools |
| :--- | :--- | :--- | :--- |
| **Student 1** | **Workforce, Competency & Certification Management**<br>• Worker profiles & badge numbers<br>• Trade qualifications (Welder, Confined Space)<br>• Certificate expiry forecasting (30-day alerts) | **Personnel & Competency Agent**<br>• Audits assigned workers against required trades<br>• Identifies expired credentials<br>• Recommends certified replacement workers | `get_worker_certificates`<br>`find_eligible_workers` |
| **Student 2** | **Safety Equipment & Isolation Management**<br>• Safety asset registry (extinguishers, gas monitors)<br>• Inspection intervals & 90-day gas calibrations<br>• Lock-Out / Tag-Out (LOTO) isolation points | **Resource & Isolation Agent**<br>• Verifies equipment inspection validity<br>• Flags overdue calibration tags<br>• Checks physical isolation points | `check_equipment_readiness`<br>`get_isolation_points` |
| **Student 3** | **Permit Lifecycle & Digital Clearance**<br>• JWT authentication, roles & login security<br>• Permit drafting, submission & QR token issuance<br>• Multi-stage status lifecycle state machine | **Planning & Coordination Agent**<br>• Decomposes permit objectives<br>• Determines permit duration envelope<br>• Enforces mandatory controls & fire watches | `get_permit_type_template` |
| **Student 4** | **Hazard Zones & Environmental SIMOPS Engine**<br>• Geospatial site zones & radius boundaries<br>• Spatial-temporal SIMOPS conflict matrix<br>• Open-Meteo meteorological integration | **Site Conditions & Hazard Control Agent**<br>• Detects adjacent zone hazards (Hot Work + Solvent)<br>• Checks live wind gusts & rainfall<br>• Recommends schedule adjustments | `get_zone_conflicts`<br>`get_weather_forecast` |
| **Shared** | **System Administration, DB & Observability**<br>• Administrator QChat Command Center<br>• Relational database seeding & recreation<br>• Trilingual i18n & Theme management | **Validation & Safety Agent**<br>• Fail-Safe consensus evaluation<br>• Hard failure packaging & remediation synthesis | `run_permit_validator` |

---

## 4. Third-Party Integration: Open-Meteo Meteorological API
- **Endpoint:** `https://api.open-meteo.com/v1/forecast`
- **Parameters:** Latitude, Longitude, Hourly Wind Speed, Hourly Wind Gusts, Rain Precipitation.
- **Safety Policy:**
  - High-risk elevated hot work and crane lifts are prohibited if wind gusts exceed **35.0 km/h**.
  - Spray painting and solvent applications are prohibited in active rain.
- **Fallback & Resilience:** If external API requests encounter timeouts or network failures, the engine enforces **Fail-Closed**: the permit escalates to manual HSE review rather than auto-clearing.
