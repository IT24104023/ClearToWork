FROM python:3.12-slim
WORKDIR /app

# Copy requirements and install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy application source code into the agents package folder
COPY . ./agents

ENV PYTHONPATH=/app
ENV PORT=8000
EXPOSE 8000

CMD ["sh", "-c", "uvicorn agents.server:app --host 0.0.0.0 --port ${PORT:-8000}"]
