# Living Lanka Marketplace — Stop All Services
# Usage: .\scripts\stop-all.ps1

Write-Host ""
Write-Host "Stopping Living Lanka services..." -ForegroundColor Yellow
Write-Host ""

# Stop dotnet processes running our APIs (by working directory pattern)
Get-CimInstance Win32_Process -Filter "Name = 'dotnet.exe'" -ErrorAction SilentlyContinue |
    Where-Object { $_.CommandLine -match 'IdentityService|ProfileService|ListingService' } |
    ForEach-Object {
        Write-Host "Stopping dotnet PID $($_.ProcessId)..." -ForegroundColor Gray
        Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue
    }

# Stop node/ng serve for living-lanka-web
Get-CimInstance Win32_Process -Filter "Name = 'node.exe'" -ErrorAction SilentlyContinue |
    Where-Object { $_.CommandLine -match 'living-lanka-web|@angular/build' } |
    ForEach-Object {
        Write-Host "Stopping node PID $($_.ProcessId)..." -ForegroundColor Gray
        Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue
    }

Write-Host ""
Write-Host "Done. Close any remaining PowerShell windows manually if needed." -ForegroundColor Green
Write-Host ""
