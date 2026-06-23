# Reset all databases — fresh seed data on next start
# Usage: npm run db:reset   OR   .\scripts\reset-databases.ps1

Write-Host ""
Write-Host "Resetting MarketPlace databases..." -ForegroundColor Yellow
Write-Host "All data will be deleted. Services must be STOPPED first." -ForegroundColor Red
Write-Host ""

$confirm = Read-Host "Type YES to continue"
if ($confirm -ne "YES") {
    Write-Host "Cancelled." -ForegroundColor Gray
    exit 0
}

$databases = @(
    "MarketPlaceIdentityDb",
    "MarketPlaceProfileDb",
    "MarketPlaceListingDb"
)

$server = "localhost"

foreach ($db in $databases) {
    Write-Host "Dropping $db..." -ForegroundColor Gray
    $query = @"
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'$db')
BEGIN
    ALTER DATABASE [$db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$db];
END
"@
    try {
        sqlcmd -S $server -E -Q $query -b 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  [OK] $db dropped" -ForegroundColor Green
        } else {
            Invoke-Sqlcmd -ServerInstance $server -Query $query -ErrorAction SilentlyContinue
            Write-Host "  [OK] $db dropped" -ForegroundColor Green
        }
    } catch {
        Write-Host "  [WARN] Could not drop $db — run manually in SSMS or ensure SQL Server is running" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Done! Now run: npm start" -ForegroundColor Green
Write-Host "Databases will be recreated with demo data automatically." -ForegroundColor Cyan
Write-Host ""
