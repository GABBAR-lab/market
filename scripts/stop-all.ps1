# Living Lanka Marketplace — Stop All Services
# Usage: npm run stop   OR   .\scripts\stop-all.ps1

$ErrorActionPreference = "SilentlyContinue"

Write-Host ""
Write-Host "Stopping Living Lanka services..." -ForegroundColor Yellow
Write-Host ""

$ports = @(5001, 5002, 5003, 4200)
$stopped = @{}

function Stop-ProcessTree {
    param([int]$ProcessId)

    if (-not $ProcessId -or $stopped.ContainsKey($ProcessId)) {
        return
    }

    $proc = Get-Process -Id $ProcessId -ErrorAction SilentlyContinue
    if (-not $proc) {
        return
    }

    Write-Host "Stopping PID $ProcessId ($($proc.ProcessName))..." -ForegroundColor Gray
    taskkill /F /PID $ProcessId /T 2>$null | Out-Null
    if (-not $?) {
        Stop-Process -Id $ProcessId -Force -ErrorAction SilentlyContinue
    }

    $stopped[$ProcessId] = $true
}

# 1) Kill whatever is listening on our ports (most reliable on Windows)
foreach ($port in $ports) {
    $connections = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    foreach ($conn in $connections) {
        Stop-ProcessTree -ProcessId $conn.OwningProcess
    }
}

Start-Sleep -Seconds 1

# 2) Kill dotnet hosts for our APIs (fallback)
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue |
    ForEach-Object {
        try {
            $cmd = (Get-CimInstance Win32_Process -Filter "ProcessId = $($_.Id)" -ErrorAction SilentlyContinue).CommandLine
            if ($cmd -match 'IdentityService|ProfileService|ListingService') {
                Stop-ProcessTree -ProcessId $_.Id
            }
        } catch {}
    }

# 3) Kill frontend dev server
Get-Process -Name "node" -ErrorAction SilentlyContinue |
    ForEach-Object {
        try {
            $cmd = (Get-CimInstance Win32_Process -Filter "ProcessId = $($_.Id)" -ErrorAction SilentlyContinue).CommandLine
            if ($cmd -match 'living-lanka-web|@angular/build|ng serve') {
                Stop-ProcessTree -ProcessId $_.Id
            }
        } catch {}
    }

Start-Sleep -Seconds 2

# 3rd pass — ports again
foreach ($port in $ports) {
    $connections = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    foreach ($conn in $connections) {
        Stop-ProcessTree -ProcessId $conn.OwningProcess
    }
}

$stillRunning = @()
foreach ($port in @(5001, 5002, 5003)) {
    if (Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue) {
        $stillRunning += $port
    }
}

if ($stillRunning.Count -gt 0) {
    Write-Host ""
    Write-Host "WARNING: Ports still in use: $($stillRunning -join ', ')" -ForegroundColor Red
    Write-Host "Close the Identity / Listing / Profile PowerShell windows manually, then run npm run stop again." -ForegroundColor Yellow
} elseif ($stopped.Count -eq 0) {
    Write-Host "No running services found on ports 5001-5003 / 4200." -ForegroundColor Gray
} else {
    Write-Host ""
    Write-Host "Stopped $($stopped.Count) process(es). Ports 5001-5003 are free." -ForegroundColor Green
}

Write-Host ""
Write-Host "You can now run: dotnet build  or  npm start" -ForegroundColor Cyan
Write-Host ""
