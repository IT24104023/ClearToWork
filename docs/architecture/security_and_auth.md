# ClearToWork AI — Security & Authentication Architecture

## 1. Authentication Strategy
ClearToWork AI utilizes stateless **JWT (JSON Web Token)** bearer authentication:
- **Algorithm:** HMAC-SHA256 (symmetric key >= 256 bits).
- **Issuer / Audience:** `ClearToWorkAPI` / `ClearToWorkClients`.
- **Expiration:** 7 days with a 5-minute clock-skew tolerance.
- **Password Security:** BCrypt hashing with auto-generated work factor salts.

---

## 2. Role-Based Access Control (RBAC) Matrix

| Operational Capability | Administrator | Safety Officer | Area Supervisor | Contractor Supervisor |
| :--- | :---: | :---: | :---: | :---: |
| **View Permits & Dashboards** | ✅ | ✅ | ✅ | ✅ |
| **Create & Submit Permits** | ✅ | ❌ | ✅ | ✅ |
| **Approve / Reject Permits** | ✅ | ✅ | ❌ | ❌ |
| **On-Site QR & GPS Activation** | ✅ | ❌ | ✅ | ✅ |
| **Close Out Permits & Evidence** | ✅ | ❌ | ✅ | ✅ |
| **Workforce Management & Certs** | ✅ | ✅ (View) | ✅ (View) | ✅ (View) |
| **Equipment & Calibration Registry** | ✅ | ✅ (Inspect) | ✅ (View) | ✅ (View) |
| **SIMOPS Conflict Rule Configuration** | ✅ | ✅ (View) | ✅ (View) | ✅ (View) |
| **QChat & Live Agent Telemetry** | ✅ | ✅ | ❌ | ❌ |
| **Database Re-Seed & Schema Reset** | ✅ | ❌ | ❌ | ❌ |

---

## 3. Defense-in-Depth & Agent Service Isolation
1. **Network Layer Isolation:**
   - The Python FastAPI LangGraph microservice is bound strictly to internal loopback / private addresses.
   - Frontend clients (React / Flutter) never communicate directly with the Python service.
2. **Cryptographic Header Verification:**
   - All requests between ASP.NET Core and the Agent service must supply an `X-Agent-Secret` header matching `ClearToWork_Internal_Agent_Key_2026`.
3. **Data Minimization:**
   - Worker identities, personal names, and contact details are never transmitted to external APIs (e.g. Open-Meteo). Only rounded geographic coordinates are exchanged.
4. **Immutable Audit Trail:**
   - All permit approvals, rejections, agent execution traces, and safe failure logs are persisted in the database with timestamps and cannot be deleted by standard users.
