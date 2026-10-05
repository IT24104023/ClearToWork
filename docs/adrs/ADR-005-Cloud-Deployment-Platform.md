# ADR-005: Containerized Microservices Deployment Strategy on Render Platform

* **Status**: ACCEPTED  
* **Date**: 2026-09-22  
* **Deciders**: ClearToWork AI Engineering Team (Zakee, Chemini, Dinithi, Oshini)  
* **Technical Area**: Cloud Deployment & DevOps (SE3090 LO4)

---

## 1. Context & Problem Statement

SE3090 requires groups to deploy the complete integrated system to a suitable cloud platform at zero subscription cost, providing working public URLs for:
- Frontend Web Client
- ASP.NET Core API Gateway with Swagger & Health URLs
- Agentic AI Service
- Standalone Runnable Android Release APK

The deployment solution needed to support Docker containerization, Git push-to-deploy automation, and predictable environment configuration.

---

## 2. Options Considered

* **Option A: Traditional VPS (AWS EC2 / DigitalOcean Droplet)**: Provides complete OS control, but requires manual Docker daemon provisioning, reverse proxy setup (Nginx SSL certbot), and incurs recurring credit card charges beyond trial credits.
* **Option B: Vercel / Netlify**: Excellent for static React frontends, but cannot natively host .NET 8 Web APIs or Python LangGraph background services.
* **Option C: Render Cloud Platform with `render.yaml` Blueprint**: Supports multi-service Docker deployments directly from a GitHub repository; offers free-tier web services, automated TLS certificate provisioning, and declarative Infrastructure-as-Code via `render.yaml`.

---

## 3. Decision

We selected **Option C: Render Cloud Platform with Declarative Blueprint (`render.yaml`)**:
1. **Frontend Web Service (`cleartowork-frontend`)**: Deployed as a lightweight Docker container running Nginx and optimized React 19 production assets.
2. **Backend API Service (`cleartowork-backend`)**: Deployed as a multi-stage .NET 8 ASP.NET Core container with exposed `/swagger`, `/health`, and `/db` endpoints.
3. **Agent Microservice (`cleartowork-agent`)**: Deployed as a Python 3.11 Docker container running FastAPI and LangGraph.
4. **Android APK Deployment**: Built via GitHub Actions and published directly to GitHub Releases (`v1.0.0-apk`) for instant download.

---

## 4. Consequences & Trade-offs

### Positive Consequences:
* Zero infrastructure cost—100% compliant with the SE3090 student budget constraint.
* Full reproducibility: The entire multi-service stack can be launched locally via `docker-compose.yml` or in the cloud via `render.yaml`.
* Automatic continuous deployment whenever commits are pushed to `main`.

### Mitigations:
* Accounted for Render free-tier sleep cycles by implementing client-side connection retry logic and responsive status indicators.
