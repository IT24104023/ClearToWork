# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ClearToWork.Domain/ClearToWork.Domain.csproj", "src/ClearToWork.Domain/"]
COPY ["src/ClearToWork.Application/ClearToWork.Application.csproj", "src/ClearToWork.Application/"]
COPY ["src/ClearToWork.Infrastructure/ClearToWork.Infrastructure.csproj", "src/ClearToWork.Infrastructure/"]
COPY ["src/ClearToWork.Api/ClearToWork.Api.csproj", "src/ClearToWork.Api/"]
RUN dotnet restore "src/ClearToWork.Api/ClearToWork.Api.csproj"
COPY src/ src/
WORKDIR "/src/src/ClearToWork.Api"
RUN dotnet publish -c Release -o /app/publish

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "ClearToWork.Api.dll"]
