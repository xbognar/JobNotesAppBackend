<<<<<<< HEAD
@echo off

REM Navigate to project directory
cd /d "path\to\your\project"

REM Stop Docker services
echo Stopping Docker services...
docker-compose down

REM Close Docker Desktop
echo Stopping Docker Desktop...
taskkill /F /IM "Docker Desktop.exe" >nul 2>&1

echo Application has been successfully stopped.
=======
@echo off

REM Navigate to project directory
cd /d "path\to\your\project"

REM Stop Docker services
echo Stopping Docker services...
docker-compose down

REM Close Docker Desktop
echo Stopping Docker Desktop...
taskkill /F /IM "Docker Desktop.exe" >nul 2>&1

echo Application has been successfully stopped.
>>>>>>> 68c53ca35de418c32724f1b55450d9d31fe874a7
