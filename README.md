# SkyRouteChallenge

SkyRoute Challenge is a full-stack flight search and booking aggregator built with a decoupled Clean Architecture backend in .NET 10 and a reactive, standalone frontend in Angular 21

## Architectural Overview (Backend)

The backend strictly follows **Clean Architecture** principles, enforcing a unidirectional dependency flow toward the core business logic.

```mermaid
graph TD
    %% Layer Definitions (Projects)
    subgraph API [Presentation Layer - SkyRoute.Api]
        Program[Program.cs / Endpoints]
        Contracts[JSON Contracts / DTOs]
    end

    subgraph Application [Application Layer - SkyRoute.Application]
        FSS[FlightSearchService]
        BS[BookingService]
        Interfaces[Core Interfaces: IFlightSearchService, IBookingService]
    end

    subgraph Infrastructure [Infrastructure Layer - SkyRoute.Infrastructure]
        DI[DependencyInjection.cs]
        subgraph Providers [Airline Provider Strategies]
            GlobalAir[GlobalAirProvider]
            BudgetWings[BudgetWingsProvider]
        end
        InfraInterfaces[Infra Interfaces: IFlightProvider]
    end

    subgraph Domain [Domain Layer - SkyRoute.Domain]
        Models[Domain Models: Flight, Booking, Passenger]
        ValueObjects[FlightSearchParams, BookingRequest]
    end

    %% Dependency Flow
    API -->|Project Reference| Application
    API -->|Project Reference| Infrastructure
    Application -->|Project Reference| Domain
    Infrastructure -->|Project Reference| Domain
    Infrastructure -.->|Implements Interfaces from| Application

    %% Layer Styling for Clear Visualization
    style Domain fill:#036a99,stroke:#01579b,stroke-width:2px;
    style Application fill:#b8754b,stroke:#1b5e20,stroke-width:2px;
    style Infrastructure fill:#ab7013,stroke:#e65100,stroke-width:2px;
    style API fill:#1c7512,stroke:#4a148c,stroke-width:2px;