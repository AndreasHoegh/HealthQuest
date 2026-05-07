#!/bin/bash

echo "🏆 HealthQuest - Quick Start Script"
echo "===================================="
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Error: Docker is not running!"
    echo "   Please start Docker Desktop and try again."
    exit 1
fi

echo "✅ Docker is running"
echo ""

# Stop any existing containers
echo "🧹 Cleaning up old containers..."
docker-compose down -v 2>/dev/null

echo ""
echo "🚀 Starting HealthQuest..."
echo ""
echo "This will start:"
echo "  - PostgreSQL database"
echo "  - Redis cache"
echo "  - .NET API"
echo ""

docker-compose up --build

# Note: The script will keep running and show logs.
# Press Ctrl+C to stop everything.
