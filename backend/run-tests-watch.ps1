# run-tests-watch.ps1
# Watches for file changes and automatically runs ALL unit tests

Write-Host "🔍 Starting Test Watcher for ALL Unit Tests..." -ForegroundColor Cyan
Write-Host "Watching for changes in src/server and tests/backend..." -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop" -ForegroundColor Gray
Write-Host ""

# Get all test projects
$testProjects = @(
    "tests\Backend\shop.Api.Tests\Shop.Api.Tests.csproj",
    "tests\Backend\Shop.Application.Tests\Shop.Application.Tests.csproj",
    "tests\Backend\Shop.Domain.Tests\Shop.Domain.Tests.csproj"
)

Write-Host "📋 Test Projects Found:" -ForegroundColor Cyan
foreach ($project in $testProjects) {
    if (Test-Path $project) {
        $projectName = Split-Path $project -Leaf
  Write-Host " ✅ $projectName" -ForegroundColor Green
    } else {
        $projectName = Split-Path $project -Leaf
        Write-Host "   ❌ $projectName (NOT FOUND)" -ForegroundColor Red
    }
}
Write-Host ""

# Initial build - Build all test projects (which will build their dependencies)
Write-Host "🔨 Building all test projects..." -ForegroundColor Cyan

$buildSuccess = $true
foreach ($project in $testProjects) {
    if (Test-Path $project) {
        $projectName = Split-Path $project -Leaf
        Write-Host "   Building $projectName..." -ForegroundColor Gray
 
        dotnet build $project --configuration Debug -v quiet
        
        if ($LASTEXITCODE -ne 0) {
       Write-Host "   ❌ Build failed for $projectName" -ForegroundColor Red
            $buildSuccess = $false
   } else {
            Write-Host "   ✅ Built successfully" -ForegroundColor Green
        }
    }
}

if (-not $buildSuccess) {
    Write-Host ""
    Write-Host "❌ Build failed! Fix errors before tests can run." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ All projects built successfully!" -ForegroundColor Green
Write-Host ""

# Initial test run for all projects
Write-Host "⚡ Running initial tests for all projects..." -ForegroundColor Green
Write-Host ""

$totalTests = 0
$totalPassed = 0
$totalFailed = 0

foreach ($project in $testProjects) {
    if (Test-Path $project) {
        $projectName = Split-Path $project -Leaf
        Write-Host "🧪 Testing: $projectName" -ForegroundColor Yellow
    
        dotnet test $project --no-build --logger "console;verbosity=minimal"
     
        if ($LASTEXITCODE -eq 0) {
            Write-Host "     ✅ Passed" -ForegroundColor Green
        } else {
            Write-Host "     ❌ Failed" -ForegroundColor Red
        }
        Write-Host ""
    }
}

Write-Host "👀 Watching for file changes..." -ForegroundColor Green
Write-Host ""

# Watch for changes
$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = (Get-Location).Path
$watcher.Filter = "*.cs"
$watcher.IncludeSubdirectories = $true
$watcher.EnableRaisingEvents = $true

$lastRun = Get-Date

$action = {
    $path = $Event.SourceEventArgs.FullPath
    $changeType = $Event.SourceEventArgs.ChangeType
    
    # Ignore changes in obj/bin/TestResults folders
    if ($path -match "\\obj\\" -or $path -match "\\bin\\" -or $path -match "\\TestResults\\") { 
        return 
    }
    
    # Debounce: wait 2 seconds between runs
    $now = Get-Date
    $timeSinceLastRun = ($now - $script:lastRun).TotalSeconds
    if ($timeSinceLastRun -lt 2) { return }
    
    $script:lastRun = $now
    
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
    Write-Host "📝 File changed: $($path.Replace($PWD.Path, '.'))" -ForegroundColor Yellow
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
    Write-Host ""
    
    # Build all test projects
    Write-Host "🔨 Rebuilding..." -ForegroundColor Cyan
    
    $buildSuccess = $true
    foreach ($project in $testProjects) {
        if (Test-Path $project) {
            dotnet build $project --configuration Debug -v quiet 2>&1 | Out-Null
            
          if ($LASTEXITCODE -ne 0) {
    $projectName = Split-Path $project -Leaf
      Write-Host "❌ Build failed for $projectName" -ForegroundColor Red
        $buildSuccess = $false
      break
    }
        }
    }
    
    if (-not $buildSuccess) {
        Write-Host "Fix errors before tests can run." -ForegroundColor Red
        return
    }
    
  Write-Host "✅ Build successful" -ForegroundColor Green
    Write-Host ""
    
  # Run all tests
  Write-Host "⚡ Re-running ALL unit tests..." -ForegroundColor Green
  Write-Host ""
    
    foreach ($project in $testProjects) {
        if (Test-Path $project) {
       $projectName = Split-Path $project -Leaf
            Write-Host "  🧪 Testing: $projectName" -ForegroundColor Yellow
 
            dotnet test $project --no-build --logger "console;verbosity=minimal" 2>&1 | Out-Null
 
   if ($LASTEXITCODE -eq 0) {
          Write-Host "✅ Passed" -ForegroundColor Green
  } else {
   Write-Host "     ❌ Failed" -ForegroundColor Red
       }
        }
    }
    
    Write-Host ""
    Write-Host "👀 Watching for changes..." -ForegroundColor Green
}

Register-ObjectEvent $watcher "Changed" -Action $action
Register-ObjectEvent $watcher "Created" -Action $action

# Keep script running
try {
    while ($true) {
    Start-Sleep -Seconds 1
    }
}
finally {
    Write-Host ""
    Write-Host "🛑 Stopping test watcher..." -ForegroundColor Yellow
    $watcher.Dispose()
}