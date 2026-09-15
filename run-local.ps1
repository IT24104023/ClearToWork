<#
.SYNOPSIS
    Starts all ClearToWork AI services locally:
    1. Python Multi-Agent Service (FastAPI / LangGraph) on http://localhost:8000
    2. Backend API (.NET 8 Web API) on http://localhost:5000
    3. Web Frontend (Vite + React) on http://localhost:5173
#>

Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "         ClearToWork AI - Local Startup             " -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan

# Configure .NET environment path
$dotnetPath = "$env:USERPROFILE\.dotnet"
if (Test-Path $dotnetPath) {
    $env:DOTNET_ROOT = $dotnetPath
    $env:PATH = "$dotnetPath;$env:PATH"
}

$rootDir = $PSScriptRoot

# 1. Start Python Agents Service
Write-Host "[1/3] Starting Python Multi-Agent Service (Port 8000)..." -ForegroundColor Yellow
Start-Process -FilePath "powershell.exe" -ArgumentList "-NoExit", "-Command", "cd '$rootDir'; python -m uvicorn agents.server:app --host 127.0.0.1 --port 8000"

# 2. Start ASP.NET Core Backend
Write-Host "[2/3] Starting ASP.NET Core Backend API (Port 5000)..." -ForegroundColor Yellow
Start-Process -FilePath "powershell.exe" -ArgumentList "-NoExit", "-Command", "`$env:DOTNET_ROOT = '$dotnetPath'; `$env:PATH = '$dotnetPath;' + `$env:PATH; cd '$rootDir'; dotnet run --project backend\src\ClearToWork.Api\ClearToWork.Api.csproj --launch-profile http"

# 3. Start Vite Frontend
Write-Host "[3/3] Starting Vite Web Frontend (Port 5173)..." -ForegroundColor Yellow
Start-Process -FilePath "powershell.exe" -ArgumentList "-NoExit", "-Command", "cd '$rootDir\web'; npm run dev -- --host 127.0.0.1 --port 5173"

Write-Host "`nAll services have been launched in separate console windows!" -ForegroundColor Green
Write-Host "Web Application:      http://localhost:5173" -ForegroundColor White
Write-Host "Backend Swagger UI:   http://localhost:5000/swagger" -ForegroundColor White
Write-Host "Agent Engine Health:  http://localhost:8000/health" -ForegroundColor White
Write-Host "`nDefault Test Logins:" -ForegroundColor Cyan
Write-Host "  - Supervisor:      supervisor@contractor.com  / Password123!" -ForegroundColor White
Write-Host "  - Safety Officer:  safety@cleartowork.com      / Password123!" -ForegroundColor White
Write-Host "  - Area Supervisor: areasup@cleartowork.com     / Password123!" -ForegroundColor White
Write-Host "  - Administrator:   admin@cleartowork.com       / Password123!" -ForegroundColor White
Write-Host ""
