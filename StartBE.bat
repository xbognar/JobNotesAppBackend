@echo off

REM Check if Docker Desktop is running
tasklist /FI "IMAGENAME eq Docker Desktop.exe" 2>NUL | find /I /N "Docker Desktop.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo Docker Desktop is already running.
) else (
    echo Starting Docker Desktop...
    REM Use PowerShell to start Docker Desktop minimized
    powershell -Command "Start-Process 'Docker Desktop.exe' -WindowStyle Minimized"
    echo Waiting for Docker Desktop to start...
)

REM Check if Docker is ready, retry if necessary
:check_docker_ready
docker info >nul 2>&1
if errorlevel 1 (
    echo Docker is not ready yet. Retrying in 5 seconds...
    timeout /t 5 >nul
    goto check_docker_ready
)

REM Navigate to project directory
cd /d "path\to\your\project"

REM Start Docker services
echo Starting Docker services...
docker-compose up --build -d

echo Application has been successfully started.

