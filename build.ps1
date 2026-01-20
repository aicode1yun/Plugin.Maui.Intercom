# Build Plugin.Maui.Intercom
# This script ensures binding projects are built before the main library

Write-Host "Building Plugin.Maui.Intercom..." -ForegroundColor Cyan
Write-Host ""

# Get script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$srcPath = Join-Path $scriptPath "src"

# Change to src directory
Push-Location $srcPath

try {
    # Step 1: Build iOS binding
    Write-Host "[1/4] Building iOS Binding..." -ForegroundColor Yellow
    dotnet build "macios\Intercom.MaciOS.Binding\Intercom.MaciOS.Binding.csproj"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: iOS Binding build failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "SUCCESS: iOS Binding built successfully" -ForegroundColor Green
    Write-Host ""

    # Step 2: Build Android binding
    Write-Host "[2/4] Building Android Binding..." -ForegroundColor Yellow
    dotnet build "android\Intercom.Android.Binding\Intercom.Android.Binding.csproj"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Android Binding build failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "SUCCESS: Android Binding built successfully" -ForegroundColor Green
    Write-Host ""

    # Step 3: Build main library
    Write-Host "[3/4] Building Main Library..." -ForegroundColor Yellow
    dotnet build "Plugin.Maui.Intercom\Plugin.Maui.Intercom.csproj"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Main Library build failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "SUCCESS: Main Library built successfully" -ForegroundColor Green
    Write-Host ""

    # Step 4: Build sample (optional)
    Write-Host "[4/4] Building Sample App..." -ForegroundColor Yellow
    dotnet build "sample\MauiSample.csproj"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "WARNING: Sample App build had issues (this is optional)" -ForegroundColor Yellow
    } else {
        Write-Host "SUCCESS: Sample App built successfully" -ForegroundColor Green
    }
    Write-Host ""

    Write-Host "Build Complete!" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now use the built libraries in your projects." -ForegroundColor Cyan

} finally {
    Pop-Location
}
