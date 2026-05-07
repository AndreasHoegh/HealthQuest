@echo off
cls
echo ==========================================
echo 🎨 HealthQuest Frontend - Starting...
echo ==========================================
echo.

cd frontend\healthquest-app

echo Checking Node.js...
node --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Node.js not found!
    echo.
    echo Please install Node.js from:
    echo https://nodejs.org/
    echo.
    pause
    exit /b 1
)

echo ✅ Node.js is installed
echo.

echo 📦 Installing dependencies...
echo This may take a few minutes on first run...
echo.

call npm install

if errorlevel 1 (
    echo ❌ npm install failed!
    pause
    exit /b 1
)

echo.
echo ✅ Dependencies installed
echo.
echo ==========================================
echo 🚀 Starting Angular Development Server...
echo ==========================================
echo.
echo 📱 Frontend will open at: http://localhost:4200
echo 🔌 Backend should be running at: http://localhost:5000
echo.
echo Press Ctrl+C to stop the server
echo ==========================================
echo.

call npm start
