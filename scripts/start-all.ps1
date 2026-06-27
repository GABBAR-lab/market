# Living Lanka Marketplace - Start All Services
# Usage: .\scripts\start-all.ps1

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Living Lanka - Starting Full Stack" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Stopping any services already on ports 5001-5003 / 4200..." -ForegroundColor Yellow
& (Join-Path $PSScriptRoot "stop-all.ps1") | Out-Null
Write-Host ""

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: .NET SDK not found. Install from https://dotnet.microsoft.com/download" -ForegroundColor Red
    exit 1
}

if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: Node.js/npm not found. Install from https://nodejs.org" -ForegroundColor Red
    exit 1
}

$webPath = Join-Path $Root "living-lanka-web"
if (-not (Test-Path (Join-Path $webPath "node_modules"))) {
    Write-Host "Installing frontend dependencies..." -ForegroundColor Yellow
    Push-Location $webPath
    npm install
    Pop-Location
}

$identityPath = Join-Path $Root "services\identity-service\src\IdentityService.Api"
$profilePath  = Join-Path $Root "services\profile-service\src\ProfileService.Api"
$listingPath  = Join-Path $Root "services\listing-service\src\ListingService.Api"

Write-Host "Starting Identity Service  -> http://localhost:5001" -ForegroundColor Green
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$identityPath'; Write-Host 'Identity Service (5001)' -ForegroundColor Magenta; dotnet run"
)

Start-Sleep -Seconds 2

Write-Host "Starting Profile Service   -> http://localhost:5002" -ForegroundColor Green
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$profilePath'; Write-Host 'Profile Service (5002)' -ForegroundColor Magenta; dotnet run"
)

Start-Sleep -Seconds 2

Write-Host "Starting Listing Service   -> http://localhost:5003" -ForegroundColor Green
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$listingPath'; Write-Host 'Listing Service (5003)' -ForegroundColor Magenta; dotnet run"
)

Start-Sleep -Seconds 3

Write-Host "Starting Frontend          -> http://localhost:4200" -ForegroundColor Green
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$webPath'; Write-Host 'Living Lanka Web (4200)' -ForegroundColor Magenta; npm start"
)

Write-Host ""
Write-Host "All services launching in separate windows..." -ForegroundColor Cyan
Write-Host "Wait ~30 seconds, then open http://localhost:4200" -ForegroundColor Yellow
Write-Host ""
Write-Host "Swagger:" -ForegroundColor White
Write-Host "  Identity: http://localhost:5001/swagger" -ForegroundColor Gray
Write-Host "  Profile:  http://localhost:5002/swagger" -ForegroundColor Gray
Write-Host "  Listing:  http://localhost:5003/swagger" -ForegroundColor Gray
Write-Host ""
Write-Host 'Login:  admin@marketplace.com  /  Admin@123' -ForegroundColor Yellow
Write-Host 'Seller: seller@marketplace.com /  Seller@123' -ForegroundColor Yellow
Write-Host "Reset:  npm run reset" -ForegroundColor Gray
Write-Host ""
