# ---- Build stage ---- (Cache Bust v1.0.1)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all csproj files first for layer caching
COPY ["src/ClearToWork.Domain/ClearToWork.Domain.csproj", "src/ClearToWork.Domain/"]
COPY ["src/ClearToWork.Application/ClearToWork.Application.csproj", "src/ClearToWork.Application/"]
COPY ["src/ClearToWork.Infrastructure/ClearToWork.Infrastructure.csproj", "src/ClearToWork.Infrastructure/"]
COPY ["src/ClearToWork.Api/ClearToWork.Api.csproj", "src/ClearToWork.Api/"]

RUN dotnet restore "src/ClearToWork.Api/ClearToWork.Api.csproj"

# Copy all source files and publish
COPY src/ src/
WORKDIR "/src/src/ClearToWork.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV PORT=5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "ClearToWork.Api.dll"]