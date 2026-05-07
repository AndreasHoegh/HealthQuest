FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY HealthQuest.API/HealthQuest.API.csproj HealthQuest.API/
COPY HealthQuest.Core/HealthQuest.Core.csproj HealthQuest.Core/
COPY HealthQuest.Infrastructure/HealthQuest.Infrastructure.csproj HealthQuest.Infrastructure/

# Restore dependencies
RUN dotnet restore HealthQuest.API/HealthQuest.API.csproj

# Copy everything else
COPY . .

# Build
WORKDIR /src/HealthQuest.API
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "HealthQuest.API.dll"]
