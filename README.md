# RealEstate API

## Overview
RealEstate API is a robust, scalable Web API built with .NET 9 for managing real estate properties in the United States. It provides endpoints to create, update, and query properties, including features like adding images, changing prices with audit traces, filtered and paginated listings, and retrieving properties by owner or with full details. The API is designed for a large real estate company, drawing from a SQL Server database schema that includes Owners, Properties, PropertyImages, and PropertyTraces.

The project emphasizes clean code, maintainability, and performance, adhering to best practices for a technical test scenario. It supports evaluation criteria such as architecture, structure, code documentation, best practices, performance management, unit testing, and security.

## Architecture
The solution follows **Clean Architecture** principles, separating concerns into independent layers to achieve high cohesion, low coupling, and easy testability. Dependencies flow inward: outer layers depend on inner ones, with the Domain at the core.

- **Domain Layer**: Contains core business entities (e.g., `Owner`, `Property`, `PropertyImage`, `PropertyTrace`), interfaces (e.g., generic `IRepository<T>`, specific `IPropertyRepository`), custom exceptions (e.g., `NotFoundException`, `ValidationException`), and business rules (e.g., factories for entity creation, validation in methods like `ChangePrice`). No external dependencies.

- **Application Layer**: Handles use cases with DTOs (e.g., `CreatePropertyDto`, `PropertyFilter`), services (e.g., `PropertyService` implementing `IPropertyService`), mappers (AutoMapper profiles), and validators (FluentValidation). Orchestrates business logic, mapping, and validation without knowing about infrastructure.

- **Infrastructure Layer**: Manages external concerns like data persistence with EF Core (`AppDbContext`), repositories (e.g., `PropertyRepository` extending `RepositoryBase<T>`), and configurations (e.g., relationships, indexes for performance).

- **API Layer**: The presentation layer with controllers (e.g., `PropertiesController`), middleware (e.g., `ExceptionMiddleware`), and startup configuration (`Program.cs`). Uses ASP.NET Core for HTTP endpoints.

This layered approach ensures the core business logic remains independent of frameworks, databases, or UI, making it easy to swap components (e.g., change ORM).

## Key Design Patterns
- **SOLID Principles**:
  - **Single Responsibility**: Each class focuses on one concern (e.g., entities handle business rules, services orchestrate use cases).
  - **Open-Closed**: Extensible via interfaces (e.g., repositories can be mocked or replaced).
  - **Liskov Substitution**: Interfaces ensure subclasses are interchangeable.
  - **Interface Segregation**: Small, focused interfaces (generic `IRepository<T>` for common CRUD, extended for entity-specific methods).
  - **Dependency Inversion**: High-level modules depend on abstractions (injected via DI in `Program.cs`).

- **Repository Pattern**: Abstracts data access with a generic `IRepository<T>` for common operations (GetById, Add, Update, SaveChanges) and specific extensions (e.g., `ListPagedAsync` for filtering/pagination). Uses `RepositoryBase<T>` for shared implementation.

- **Unit of Work**: Handled via EF Core's `SaveChangesAsync` in repositories, ensuring atomic operations.

- **Factory Pattern**: Entities use static `Create` methods for validation and immutability.

- **CQRS-Like Separation**: Services separate commands (Create, Update) from queries (List, GetDetails), though not full CQRS.

- **Middleware Pattern**: Custom `ExceptionMiddleware` for global error handling.

Decisions: Generic repositories reduce code duplication while allowing entity-specific extensions. Async operations everywhere for non-blocking I/O.

## Technologies and Libraries
- **Framework**: .NET 9 (for modern features like built-in OpenAPI support).
- **ORM**: Entity Framework Core (for SQL Server integration, migrations, and querying).
- **Mapping**: AutoMapper (for Entity ↔ DTO conversions).
- **Validation**: FluentValidation (for input DTOs, e.g., price > 0, ranges in filters).
- **Logging**: Serilog (structured logging with console/file sinks, enrichers for context).
- **Testing**: nUnit (unit tests), Moq (mocking dependencies).
- **API Docs**: Built-in OpenAPI in .NET 9 (`Microsoft.AspNetCore.OpenApi`) for JSON generation, Swagger UI for visualization (with XML comments enabled).
- **Other**: System.Text.Json (default serialization), Microsoft.Extensions.DependencyInjection (DI).

Decisions: Chose EF Core for its LINQ querying and performance features (e.g., `AsNoTracking`, indexes). Serilog over default logging for structured output. nUnit as specified. Migrated to .NET 9's native OpenAPI for lighter weight and AOT compatibility, avoiding full Swashbuckle.

## Features
- **Property Management**:
  - Create a new property (POST /api/properties).
  - Update property details (PUT /api/properties/{id}).
  - Change price with audit trace (PATCH /api/properties/{id}/price).
  - Add image to property (POST /api/properties/{id}/images).

- **Querying**:
  - List properties with filters (name, address, code internal, price/year ranges, owner ID) and pagination (GET /api/properties).
  - Get properties by owner (GET /api/properties/owner/{ownerId}/properties).
  - Get property details including owner, images, traces (GET /api/properties/{id}/details).

- **Bidirectional Navigation**: Owners can list their properties; properties include owner details.

- **Performance Optimizations**: Eager loading with `Include`, projections, database indexes on filtered columns, `AsNoTracking` for reads.

- **Security**: Input validation, custom exceptions, middleware for error handling (returns JSON errors), parameterized queries (EF default).

- **Documentation**: XML comments on controllers/DTOs for Swagger UI.

- **Logging**: Structured logs for operations (e.g., "Creating property {Name}").

## Setup and Running
1. **Prerequisites**: .NET 9 SDK, SQL Server (localdb or instance).
2. **Clone and Build**: `dotnet restore`, `dotnet build`.
3. **Database**: Update connection string in `appsettings.json`. Run migrations: `dotnet ef migrations add Initial`, `dotnet ef database update`.
4. **Run**: `dotnet run` (runs on https://localhost:5015 or configured port).
5. **Swagger UI**: Available at root URL in development (e.g., https://localhost:5015/) with full docs.

## Testing
- **Unit Tests**: In `RealEstate.Tests` project using nUnit. Covers services (e.g., create, list with filters, exceptions). Mocks repositories/mappers/logger.
- **Run Tests**: `dotnet test`.
- Coverage: Focuses on happy paths, edge cases (e.g., invalid filters throw `ValidationException`), and verifications (e.g., repo calls).

Decisions: Used Moq for isolation. Tests are async where applicable.

## Interesting Aspects
- **Immutability in Entities**: Properties are private-set, with methods/factories for changes, enforcing business rules (e.g., price > 0).
- **Audit Traces**: Price changes automatically add `PropertyTrace` for history, promoting data integrity.
- **Pagination and Filtering**: Dynamic EF queries with `IQueryable` for efficient DB hits; validation prevents invalid ranges.
- **Error Handling**: Custom middleware transforms exceptions into user-friendly JSON responses (e.g., 404 for not found).
- **OpenAPI Migration**: Switched to .NET 9's native OpenAPI for better performance/AOT support, with XML docs auto-included via analyzers.
- **Structured Logging**: Serilog captures context (e.g., PropertyId) for traceability.
- **Test-Driven Elements**: Tests influenced service design (e.g., injectable validators).

This API is production-ready for the test requirements, with room for extensions like authentication (JWT) or caching. Contributions welcome!