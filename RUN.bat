@echo off
cls
echo ==========================================
echo 🏆 HealthQuest API - Quick Start
echo ==========================================
echo.

echo Checking .NET installation...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET 8 SDK not found!
    echo.
    echo Please install .NET 8 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

echo ✅ .NET is installed
echo.

cd backend\HealthQuest.API

echo 📦 Restoring packages...
dotnet restore --verbosity quiet

if errorlevel 1 (
    echo ❌ Package restore failed!
    echo.
    echo Try running this in Command Prompt:
    echo cd backend\HealthQuest.API
    echo dotnet restore
    echo.
    pause
    exit /b 1
)

echo ✅ Packages restored
echo.

echo 🔨 Building project...
dotnet build --configuration Release --verbosity quiet

if errorlevel 1 (
    echo ❌ Build failed!
    echo.
    echo Let me show you the full error:
    echo.
    dotnet build --configuration Release
    echo.
    pause
    exit /b 1
)

echo ✅ Build successful
echo.
echo ==========================================
echo 🚀 Starting HealthQuest API...
echo ==========================================
echo.
echo ✅ Test users auto-created:
echo    📧 test@healthquest.dk / Test123!
echo    📧 doctor@healthquest.dk / Doctor123!
echo.
echo 📖 Swagger UI: http://localhost:5000/swagger
echo.
echo Press Ctrl+C to stop the server
echo ==========================================
echo.

dotnet run --no-build --configuration Release
