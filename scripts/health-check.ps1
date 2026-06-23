# Living Lanka Marketplace — Health Check
# Usage: .\scripts\health-check.ps1

Write-Host ""
Write-Host "Checking services..." -ForegroundColor Cyan
Write-Host ""

$services = @(
    @{ Name = "Identity Service"; Url = "http://localhost:5001/api/health" },
    @{ Name = "Profile Service";  Url = "http://localhost:5002/api/health" },
    @{ Name = "Listing Service"; Url = "http://localhost:5003/api/health" },
    @{ Name = "Frontend";        Url = "http://localhost:4200" }
)

$allOk = $true

foreach ($svc in $services) {
    try {
        $response = Invoke-WebRequest -Uri $svc.Url -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            Write-Host "  [OK]   $($svc.Name) — $($svc.Url)" -ForegroundColor Green
        } else {
            Write-Host "  [FAIL] $($svc.Name) — Status $($response.StatusCode)" -ForegroundColor Red
            $allOk = $false
        }
    } catch {
        Write-Host "  [DOWN] $($svc.Name) — $($svc.Url)" -ForegroundColor Red
        $allOk = $false
    }
}

Write-Host ""
if ($allOk) {
    Write-Host "All services are running!" -ForegroundColor Green
    Write-Host "Open http://localhost:4200 in your browser." -ForegroundColor Yellow
} else {
    Write-Host "Some services are not running yet." -ForegroundColor Yellow
    Write-Host "Run: .\scripts\start-all.ps1" -ForegroundColor Gray
    Write-Host "Wait 30-60 seconds and try again." -ForegroundColor Gray
}
Write-Host ""
