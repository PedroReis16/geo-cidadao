# Geo Cidadão UI Docker Management Script for PowerShell

param(
    [Parameter(Position=0)]
    [string]$Command = "help"
)

# Function to print colored output
function Write-Status {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Function to check if Docker is running
function Test-Docker {
    try {
        docker info | Out-Null
        return $true
    } catch {
        Write-Error "Docker is not running. Please start Docker and try again."
        return $false
    }
}

# Function to build the UI container
function Invoke-UIBuild {
    Write-Status "Building Geo Cidadão UI container..."
    docker-compose build geo-cidadao-ui
    if ($LASTEXITCODE -eq 0) {
        Write-Success "UI container built successfully!"
    } else {
        Write-Error "Failed to build UI container."
        exit 1
    }
}

# Function to start the UI service
function Start-UI {
    Write-Status "Starting Geo Cidadão UI service..."
    docker-compose up -d geo-cidadao-ui
    if ($LASTEXITCODE -eq 0) {
        Write-Success "UI service started successfully!"
        Write-Status "UI is available at: http://localhost:3000"
        Write-Status "UI through Traefik: http://localhost:81"
    } else {
        Write-Error "Failed to start UI service."
        exit 1
    }
}

# Function to stop the UI service
function Stop-UI {
    Write-Status "Stopping Geo Cidadão UI service..."
    docker-compose stop geo-cidadao-ui
    if ($LASTEXITCODE -eq 0) {
        Write-Success "UI service stopped successfully!"
    } else {
        Write-Error "Failed to stop UI service."
        exit 1
    }
}

# Function to restart the UI service
function Restart-UI {
    Write-Status "Restarting Geo Cidadão UI service..."
    docker-compose restart geo-cidadao-ui
    if ($LASTEXITCODE -eq 0) {
        Write-Success "UI service restarted successfully!"
    } else {
        Write-Error "Failed to restart UI service."
        exit 1
    }
}

# Function to view logs
function Show-Logs {
    Write-Status "Showing Geo Cidadão UI logs..."
    docker-compose logs -f geo-cidadao-ui
}

# Function to remove UI container and image
function Remove-UIContainer {
    Write-Warning "This will remove the UI container and image. Are you sure? (y/N)"
    $response = Read-Host
    if ($response -match '^[yY]([eE][sS])?$') {
        Write-Status "Stopping and removing UI container..."
        docker-compose down geo-cidadao-ui
        docker rmi geo-cidadao_geo-cidadao-ui 2>$null
        Write-Success "UI container and image removed successfully!"
    } else {
        Write-Status "Operation cancelled."
    }
}

# Function to show help
function Show-Help {
    Write-Host "Geo Cidadão UI Docker Management Script" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\ui-docker.ps1 [COMMAND]" -ForegroundColor White
    Write-Host ""
    Write-Host "Commands:" -ForegroundColor White
    Write-Host "  build    Build the UI container" -ForegroundColor Gray
    Write-Host "  start    Start the UI service" -ForegroundColor Gray
    Write-Host "  stop     Stop the UI service" -ForegroundColor Gray
    Write-Host "  restart  Restart the UI service" -ForegroundColor Gray
    Write-Host "  logs     Show UI service logs" -ForegroundColor Gray
    Write-Host "  clean    Remove UI container and image" -ForegroundColor Gray
    Write-Host "  help     Show this help message" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor White
    Write-Host "  .\ui-docker.ps1 build" -ForegroundColor Gray
    Write-Host "  .\ui-docker.ps1 start" -ForegroundColor Gray
    Write-Host "  .\ui-docker.ps1 logs" -ForegroundColor Gray
}

# Main script logic
if (-not (Test-Docker)) {
    exit 1
}

switch ($Command.ToLower()) {
    "build" {
        Invoke-UIBuild
    }
    "start" {
        Start-UI
    }
    "stop" {
        Stop-UI
    }
    "restart" {
        Restart-UI
    }
    "logs" {
        Show-Logs
    }
    "clean" {
        Remove-UIContainer
    }
    "help" {
        Show-Help
    }
    default {
        Write-Error "Unknown command: $Command"
        Show-Help
        exit 1
    }
}