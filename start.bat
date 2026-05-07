@echo off
echo ======================================
echo 🏆 HealthQuest - Quick Start Script
echo ======================================
echo.

echo Checking Docker...
docker info >nul 2>&1
if errorlevel 1 (
    echo ❌ Error: Docker is not running!
    echo    Please start Docker Desktop and try again.
    pause
    exit /b 1
)

echo ✅ Docker is running
echo.

echo 🧹 Cleaning up old containers...
docker-compose down -v 2>nul

echo.
echo 🚀 Starting HealthQuest...
echo.
echo This will start:
echo   - PostgreSQL database
echo   - Redis cache
echo   - .NET API
echo.
echo Press Ctrl+C to stop everything.
echo.

docker-compose up --build
