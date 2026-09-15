# Dockerfile for ClearToWork.Api (ASP.NET Core 8)

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClearToWork.Api/ClearToWork.Api.csproj", "ClearToWork.Api/"]
RUN dotnet restore "ClearToWork.Api/ClearToWork.Api.csproj"
COPY . .
WORKDIR "/src/ClearToWork.Api"
RUN dotnet publish -c Release -o /app/publish

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "ClearToWork.Api.dll"]
