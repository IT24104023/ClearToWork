# Dockerfile for agents FastAPI service
FROM python:3.12-slim AS base
WORKDIR /app
# Install uv (fast installer) and use it to install deps
RUN pip install --no-cache-dir uv && \
    uv pip install --no-cache-dir -r requirements.txt
COPY . .
EXPOSE 8000
CMD ["uvicorn", "agents.server:app", "--host", "0.0.0.0", "--port", "8000"]
