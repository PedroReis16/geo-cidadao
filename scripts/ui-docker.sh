#!/bin/bash

# Geo Cidadão UI Docker Management Script

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if Docker is running
check_docker() {
    if ! docker info >/dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
}

# Function to build the UI container
build_ui() {
    print_status "Building Geo Cidadão UI container..."
    docker-compose build geo-cidadao-ui
    print_success "UI container built successfully!"
}

# Function to start the UI service
start_ui() {
    print_status "Starting Geo Cidadão UI service..."
    docker-compose up -d geo-cidadao-ui
    print_success "UI service started successfully!"
    print_status "UI is available at: http://localhost:3000"
    print_status "UI through Traefik: http://localhost:81"
}

# Function to stop the UI service
stop_ui() {
    print_status "Stopping Geo Cidadão UI service..."
    docker-compose stop geo-cidadao-ui
    print_success "UI service stopped successfully!"
}

# Function to restart the UI service
restart_ui() {
    print_status "Restarting Geo Cidadão UI service..."
    docker-compose restart geo-cidadao-ui
    print_success "UI service restarted successfully!"
}

# Function to view logs
logs_ui() {
    print_status "Showing Geo Cidadão UI logs..."
    docker-compose logs -f geo-cidadao-ui
}

# Function to remove UI container and image
clean_ui() {
    print_warning "This will remove the UI container and image. Are you sure? (y/N)"
    read -r response
    if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
        print_status "Stopping and removing UI container..."
        docker-compose down geo-cidadao-ui
        docker rmi geo-cidadao_geo-cidadao-ui 2>/dev/null || true
        print_success "UI container and image removed successfully!"
    else
        print_status "Operation cancelled."
    fi
}

# Function to show help
show_help() {
    echo "Geo Cidadão UI Docker Management Script"
    echo ""
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  build    Build the UI container"
    echo "  start    Start the UI service"
    echo "  stop     Stop the UI service"
    echo "  restart  Restart the UI service"
    echo "  logs     Show UI service logs"
    echo "  clean    Remove UI container and image"
    echo "  help     Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 build"
    echo "  $0 start"
    echo "  $0 logs"
}

# Main script logic
main() {
    check_docker
    
    case "${1:-help}" in
        build)
            build_ui
            ;;
        start)
            start_ui
            ;;
        stop)
            stop_ui
            ;;
        restart)
            restart_ui
            ;;
        logs)
            logs_ui
            ;;
        clean)
            clean_ui
            ;;
        help|--help|-h)
            show_help
            ;;
        *)
            print_error "Unknown command: $1"
            show_help
            exit 1
            ;;
    esac
}

# Run main function
main "$@"