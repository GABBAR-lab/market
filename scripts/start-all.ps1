# Living Lanka Marketplace - Start All Services
# Usage: .\scripts\start-all.ps1

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Living Lanka - Starting Full Stack" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Stopping any services already on ports 5001-5009 / 4200..." -ForegroundColor Yellow
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

$services = @(
    @{ Name = "Identity Service";     Port = 5001; Path = "services\identity-service\src\IdentityService.Api" },
    @{ Name = "Profile Service";      Port = 5002; Path = "services\profile-service\src\ProfileService.Api" },
    @{ Name = "Listing Service";      Port = 5003; Path = "services\listing-service\src\ListingService.Api" },
    @{ Name = "Media Service";        Port = 5004; Path = "services\media-service\src\MediaService.Api" },
    @{ Name = "Payment Service";      Port = 5005; Path = "services\payment-service\src\PaymentService.Api" },
    @{ Name = "Notification Service"; Port = 5006; Path = "services\notification-service\src\NotificationService.Api" },
    @{ Name = "Chat Service";         Port = 5007; Path = "services\chat-service\src\ChatService.Api" },
    @{ Name = "Logging Service";      Port = 5008; Path = "services\logging-service\src\LoggingService.Api" },
    @{ Name = "API Gateway";          Port = 5009; Path = "services\gateway\src\MarketPlace.Gateway" }
)

foreach ($svc in $services) {
    $fullPath = Join-Path $Root $svc.Path
    Write-Host "Starting $($svc.Name.PadRight(22)) -> http://localhost:$($svc.Port)" -ForegroundColor Green
    Start-Process powershell -ArgumentList @(
        "-NoExit", "-Command",
        "cd '$fullPath'; Write-Host '$($svc.Name) ($($svc.Port))' -ForegroundColor Magenta; dotnet run"
    )
    Start-Sleep -Seconds 2
}

Write-Host "Starting Frontend          -> http://localhost:4200" -ForegroundColor Green
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$webPath'; Write-Host 'Living Lanka Web (4200)' -ForegroundColor Magenta; npm start"
)

Write-Host ""
Write-Host "All services launching in separate windows..." -ForegroundColor Cyan
Write-Host "Wait ~45 seconds, then open http://localhost:4200" -ForegroundColor Yellow
Write-Host ""
Write-Host "API Gateway: http://localhost:5009" -ForegroundColor White
Write-Host "RabbitMQ UI: http://localhost:15672 (guest/guest)" -ForegroundColor Gray
Write-Host ""
Write-Host 'Login:  admin@marketplace.com  /  Admin@123' -ForegroundColor Yellow
Write-Host 'Seller: seller@marketplace.com /  Seller@123' -ForegroundColor Yellow
Write-Host "Infra:  npm run sql  (SQL Server + Redis + RabbitMQ)" -ForegroundColor Gray
Write-Host "Reset:  npm run reset" -ForegroundColor Gray
Write-Host ""
