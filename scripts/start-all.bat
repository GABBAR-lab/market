@echo off
REM Living Lanka — Start All (double-click friendly)
cd /d "%~dp0.."
powershell -ExecutionPolicy Bypass -File "%~dp0start-all.ps1"
pause
