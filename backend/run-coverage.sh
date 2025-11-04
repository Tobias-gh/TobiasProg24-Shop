#!/bin/bash

echo "Running tests with code coverage..."

# Clean previous coverage results
echo ""
echo "Cleaning previous coverage results..."
rm -rf TestResults
rm -rf coveragereport

# Run tests with coverage collection
echo ""
echo "Running tests with coverage collection..."
dotnet test --collect:"XPlat Code Coverage" --settings:coverlet.runsettings --results-directory:"./TestResults"

# Check if coverage files were generated
if [ ! -d "TestResults" ]; then
    echo ""
    echo "Error: No coverage files generated!"
    exit 1
fi

# Install ReportGenerator if not already installed
echo ""
echo "Checking for ReportGenerator..."
if ! dotnet tool list -g | grep -q "dotnet-reportgenerator-globaltool"; then
    echo "Installing ReportGenerator..."
    dotnet tool install -g dotnet-reportgenerator-globaltool
else
  echo "ReportGenerator already installed"
fi

# Generate HTML report with class filters to exclude infrastructure
echo ""
echo "Generating HTML coverage report (excluding migrations, configurations, seeders)..."
reportgenerator \
    -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
    -reporttypes:"Html;Cobertura;TextSummary;Badges" \
    -classfilters:"-Shop.Infrastructure.Migrations.*;-Shop.Infrastructure.Data.Configurations.*;-Shop.Infrastructure.Data.Seeders.*;-Shop.Infrastructure.Data.ShopDbContext"

# Display summary
echo ""
echo "========================================"
echo "Coverage Report Generated Successfully!"
echo "========================================"
echo ""

# Read and display summary
if [ -f "coveragereport/Summary.txt" ]; then
    echo "Coverage Summary:"
    cat coveragereport/Summary.txt
fi

echo ""
echo "========================================"
echo "Files Generated:"
echo "  - HTML Report: coveragereport/index.html"
echo "  - Coverage Badge: coveragereport/badge_linecoverage.svg"
echo "  - XML Report: coveragereport/Cobertura.xml"
echo "========================================"
echo ""

echo "Note: Infrastructure (migrations, configs, seeders) excluded from coverage"
echo ""
echo "To open the report:"
echo "  Mac:   open coveragereport/index.html"
echo "  Linux: xdg-open coveragereport/index.html"
echo ""
echo "Done!"
