# Run Tests with Code Coverage

Write-Host "Running tests with code coverage..." -ForegroundColor Green

# Clean previous coverage results
Write-Host "`nCleaning previous coverage results..." -ForegroundColor Yellow
if (Test-Path "TestResults") {
    Remove-Item -Path "TestResults" -Recurse -Force
}
if (Test-Path "coveragereport") {
    Remove-Item -Path "coveragereport" -Recurse -Force
}

# Run tests with coverage collection
Write-Host "`nRunning tests with coverage collection..." -ForegroundColor Yellow
dotnet test --collect:"XPlat Code Coverage" --settings:coverlet.runsettings --results-directory:"./TestResults"

# Check if coverage files were generated
$coverageFiles = Get-ChildItem -Path "TestResults" -Filter "coverage.cobertura.xml" -Recurse

if ($coverageFiles.Count -eq 0) {
    Write-Host "`nError: No coverage files generated!" -ForegroundColor Red
    exit 1
}

Write-Host "`nFound $($coverageFiles.Count) coverage file(s)" -ForegroundColor Green

# Install ReportGenerator if not already installed
Write-Host "`nChecking for ReportGenerator..." -ForegroundColor Yellow
$reportGenerator = dotnet tool list -g | Select-String "dotnet-reportgenerator-globaltool"

if (-not $reportGenerator) {
    Write-Host "Installing ReportGenerator..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
} else {
    Write-Host "ReportGenerator already installed" -ForegroundColor Green
}

# Generate HTML report with class filters to exclude infrastructure
Write-Host "`nGenerating HTML coverage report (excluding migrations, configurations, seeders)..." -ForegroundColor Yellow
reportgenerator `
 -reports:"TestResults/**/coverage.cobertura.xml" `
    -targetdir:"coveragereport" `
    -reporttypes:"Html;Cobertura;TextSummary;Badges" `
    -classfilters:"-Shop.Infrastructure.Migrations.*;-Shop.Infrastructure.Data.Configurations.*;-Shop.Infrastructure.Data.Seeders.*;-Shop.Infrastructure.Data.ShopDbContext"

# Display summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Coverage Report Generated Successfully!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Cyan

# Read and display summary
if (Test-Path "coveragereport/Summary.txt") {
    Write-Host "Coverage Summary:" -ForegroundColor Yellow
    Get-Content "coveragereport/Summary.txt"
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Files Generated:" -ForegroundColor Yellow
Write-Host "  - HTML Report: coveragereport/index.html" -ForegroundColor White
Write-Host "  - Coverage Badge: coveragereport/badge_linecoverage.svg" -ForegroundColor White
Write-Host "  - XML Report: coveragereport/Cobertura.xml" -ForegroundColor White
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Note: Infrastructure (migrations, configs, seeders) excluded from coverage" -ForegroundColor Yellow
Write-Host "Opening report in browser..." -ForegroundColor Yellow

# Open the report in default browser
Start-Process "coveragereport/index.html"

Write-Host "`nDone!" -ForegroundColor Green
