# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy all project files
COPY . .

# Publish Api project regardless of whether build context is repo root or backend folder
RUN if [ -f "src/ClearToWork.Api/ClearToWork.Api.csproj" ]; then \
        dotnet publish "src/ClearToWork.Api/ClearToWork.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false; \
    else \
        dotnet publish "backend/src/ClearToWork.Api/ClearToWork.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false; \
    fi

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV PORT=5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "ClearToWork.Api.dll"]