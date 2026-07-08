# Release Notes - Kitchen.Api

[//]: ## '[Version X] - YYYY-MM-DD'
[//]: ### 'Summary'


-------------------------
## [1.8.0] - 2026-07-09
### Expose GET by Name for ProductDefinitions

| Layer | Details |
| :--- | :--- |
| **Api** | • Added `GET /api/productdefinitions/{name}`, wrapping the `ICatalogService.GetByName()` that already existed in the application layer but wasn't reachable from the outside. |
| **Api** | • `POST /api/productdefinitions` now returns a `Location` header pointing at the new `Get` action instead of `GetAll` — a real self-link instead of pointing at the collection. |
| **Test** | • Updated `Create_ShouldReturnCreatedAtAction_WhenRequestIsValid` for the new action name; added `Get_ShouldReturnOk_WhenProductDefinitionExists` / `Get_ShouldReturnNotFound_WhenProductDefinitionDoesNotExist`. |

-------------------------
## [1.7.0] - 2026-07-08
### Fix JSON Enum Serialization

| Layer | Details |
| :--- | :--- |
| **Core** | • `StockItem.Location` now has `[JsonConverter(typeof(JsonStringEnumConverter))]` — was missing it, so `GET` responses returned it as a raw integer while `POST`/`PUT` requests took a string for the same field. |
| **Application** | • Removed the `[JsonConverter(typeof(JsonStringEnumConverter))]` attribute from `ProductDefinition.Unit` and `Create-`/`UpdateProductDefinitionRequest.Unit` — that attribute was shadowing the globally-registered `UnitTypeConverter` (property-level `[JsonConverter]` always wins over a converter in `JsonSerializerOptions.Converters`), so the Polish unit aliases (`szt`, `kg`, `l`, `litry`) never actually worked. |
| **Docs** | • `docs/api.md` updated to describe the corrected behavior — including a newly-noticed side effect: `UnitTypeConverter.Read` silently falls back to `Unspecified` for unrecognized input instead of throwing, so a typo in `unit` no longer produces a `400`. |

-------------------------
## [1.6.0] - 2026-07-06
### Expiration Dates & Definition Rename

| Layer | Details |
| :--- | :--- |
| **Core** | • Added nullable `ExpirationDate` to `StockItem` (`SetExpirationDate`), intentionally without a "not in the past" validation — an expired item is still a real item.<br>• Renamed `StockItem.Type` → `StockItem.Definition` / `TypeName` → `DefinitionName` across the domain and EF configuration, to match `ProductDefinition` naming. |
| **Application** | • `AddStockItemCommand`/`ModifyStockItemCommand` now carry `ExpirationDate`.<br>• Fixed `CatalogService.LinkToExistingStockItems` re-inserting a `ProductDefinition` that a `StockItem` already referenced. |
| **Infrastructure** | • Added `RenameTypeToDefinition` migration.<br>• `ExceptionMiddleware` now logs unhandled exceptions before returning `500`. |
| **Test** | • Added integration coverage for `ProductDefinition` ↔ `StockItem` auto-linking.<br>• Regression test for a bug where `ProductName` compared by reference instead of value, silently breaking auto-linking.<br>• Fixed a missing logger mock in `ExceptionMiddlewareTests`. |
| **DevOps** | • Added `.gitattributes` to normalize line endings. |

-------------------------
## [1.5.0] - 2026-07-05
### Integration Testing & CI

| Layer | Details |
| :--- | :--- |
| **Infrastructure** | • Squashed all EF Core migrations into a single `Initial` baseline. |
| **Test** | • Added `Kitchen.Tests.Integration` — repository tests against a real PostgreSQL container via **Testcontainers**, running actual migrations instead of `EnsureCreated()`. |
| **DevOps** | • Added a GitHub Actions workflow: build + unit tests + integration tests on PRs to `master` and pushes to `dev`.<br>• Removed the gitignored `Private` project from the solution file.<br>• Dev environment fixes and a more realistic seed dataset. |

-------------------------
## [1.4.0] - 2026-07-04
### Exception Middleware Rewrite & CORS

| Layer | Details |
| :--- | :--- |
| **Infrastructure** | • Rewrote the global exception handler as `ExceptionMiddleware : IMiddleware`, moved from `Kitchen.Api` to `Kitchen.Infrastructure`.<br>• JSON error body changed to `{ "code", "message" }`, with `code` derived from the exception type name. |
| **Core** | • Reorganized exception files; added dedicated handling for `UnknownCategoryException`. |
| **Api** | • Added a CORS policy (`FrontendCorsPolicy`) allowing `http://localhost:5173` for local frontend development. |

-------------------------
## [1.3.0] - 2026-06-14
### GUID Identity & Async Everywhere

| Layer | Details |
| :--- | :--- |
| **Core** | • Switched `StockItem`'s identity from name-based to a GUID `StockItemId`.<br>• Fixed assorted runtime bugs; tightened encapsulation. |
| **All** | • Converted the full call chain — controllers, services, repositories — to `async`/`await`. |
| **Test** | • Updated unit tests for the async refactor. |

-------------------------
## [1.2.0] - 2026-05-13
### Domain Rename: Ingredient → StockItem / ProductDefinition

| Layer | Details |
| :--- | :--- |
| **Core** | • Renamed `Ingredient` → `StockItem` and `IngredientType` → `ProductDefinition` across the codebase to better reflect the domain language. |
| **Application** | • `StockItem`s eagerly load their linked `ProductDefinition` via `*WithDetails` repository methods. |

-------------------------
## [1.1.0] - 2026-05-07
### Categories, Polish Unit Aliases & Stronger Validation

| Layer | Details |
| :--- | :--- |
| **Core** | • Added the `Category` enum (`Meat`, `Vegetables`, `Dairy`, `DryGoods`, `Spices`, `Other`) on `ProductDefinition`.<br>• `UnitTypeConverter` accepts Polish aliases (`szt`, `sztuk`, `kg`, `l`, `litry`, ...) on input. |
| **Application/Api** | • Fixed a bug in the update flow; added structured domain exception handling. |
| **Test** | • Expanded validation and service test coverage. |

-----------------------------------------------------------
## [1.0.0] - Release 1: "The Digital Pantry" - 2026-05-02
-----------------------------------------------------------

### Purpose & Business Value
The primary objective of this release is to deliver a reliable tracking system for household food supplies. By maintaining an accurate, real-time record of ingredients across various storage locations, the system eliminates the need for manual tracking and contributes to the reduction of food waste.

### Features & Scope
The initial release establishes the foundational ecosystem for digital pantry management:
* **Inventory Management:** Full CRUD (Create, Read, Update, Delete) capabilities for food items, with support for specific physical assignments: Fridge, Freezer, and Pantry.
* **Product Catalog:** A standardized dictionary of ingredient types to ensure data consistency and provide metadata for all products.

### Architecture & Technical Design
The system is implemented following **Clean Architecture** and **Domain-Driven Design (DDD)** principles to ensure long-term maintainability and scalability.

#### 1. Layer Responsibilities
* **Kitchen.Core (Domain):** The central layer containing pure business logic, entities, and value objects. It ensures data integrity by enforcing essential business rules directly within the models.
* **Kitchen.Application:** An orchestration layer that translates user intents into business actions (Commands) and coordinates the execution flow.
* **Kitchen.Infrastructure:** Manages technical implementation details, including PostgreSQL communication via Entity Framework Core.
* **Kitchen.Api (Presentation):** The system's entry point, exposing RESTful endpoints with integrated OpenAPI (Swagger) documentation.

#### 2. Technical Foundation
* **Data Persistence:** PostgreSQL database integrated with `docker-compose.yml` for consistent local and production environments.
* **Automated Lifecycle:** Implementation of automated migrations and data seeding on application startup for seamless deployment.
* **Security & Config:** Externalized configuration management using `appsettings.json` to secure connection strings and environment settings.
* **Quality Assurance:** A dedicated suite of unit tests (xUnit & FluentAssertions) verifying both domain rules and application-level orchestration.

-------------------------
## [0.0.8] - 2026-04-30
### PostgreSQL Database Integration

| Layer | Details |
| :--- | :--- |
| **Core** | • Added private constructors to domain entities to ensure EF Core compatibility. |
| **Application** | • Updated service lifetimes from `Singleton` to `Scoped` to ensure database context safety per request. |
| **Infrastructure** | • Integrated **PostgreSQL** via EF Core as the primary database, replacing all in-memory collections.<br>• Added `KitchenDbContext` with Fluent API configurations.<br>• Added automated migrations and data seeding; supported via `IDesignTimeDbContextFactory`. |
| **DevOps** | • Added `docker-compose.yml` for database setup.<br>• Moved database configuration to `appsettings.json`. |

-------------------------
## [0.0.7] - 2026-04-25
### Cleanup - DI Standardisation, Internal Access & CRUD in Testing

| Layer | Details |
| :--- | :--- |
| **All** | • Cleaned up Dependency Injection by standardizing registration classes as `Extensions.cs` in all projects. |
| **Test** | • Added `InternalsVisibleTo` to give unit tests access to `internal` classes while keeping them hidden from other projects.<br>• Verified CRUD lifecycle in InventoryService via unit tests. |

-------------------------
## [0.0.6] - 2026-04-24
### Multi-Layered Clean Architecture

| Aspect | Details |
| :--- | :--- |
| **Project Structure** | Split the monolith into a multi-project solution to prevent leaky abstractions. |
| **Dependency Flow** | Enforced a strict one-way dependency flow pointing directly towards the domain core. |
| **Visibility** | Applied the `internal` modifier to implementations, exposing only interfaces to the outside. |

| Layer | Details |
| :--- | :--- |
| **Core** | Created domain layer containing core entities, value objects, and repository interfaces. |
| **Application** | Created application layer to handle business logic via services and commands. |
| **Infrastructure** | Created infrastructure layer for technical implementations like data access. |

-------------------------
## [0.0.5] - 2026-04-23
### Repository Pattern & Service Unit Testing

| Layer | Details |
| :--- | :--- |
| **Core** | • Introduced repository contracts to separate data access from core business logic.<br>• Renamed `IngredientDefinition` to `IngredientType` (Catalog). |
| **Application** | • Moved data management to Repositories; Services now focus on logic and errors. |
| **API** | • Simplified controllers by using exception-based flow. |
| **Infrastructure** | • Created temporary in-memory repositories. |
| **Test** | • Added Service unit tests with Moq for repository isolation. |

-------------------------
## [0.0.4] - 2026-04-16
### Commands & Unit Tests

| Layer | Details |
| :--- | :--- |
| **Application** | • Refined the command pattern by grouping actions into `InventoryCommands` and `CatalogCommands`.<br>• Partial updates: added dedicated requests with nullable types. |
| **Core** | • Optional updates: Switched to nullable types in Domain methods. |
| **API** | • Refactored endpoints to accept commands (instead of raw entities).<br>• Improved POST endpoints to return `201 Created` with a valid location header. |
| **Test** | • Added unit test project with Xunit and FluentAssertions.<br>• Added comprehensive unit tests to verify state transitions in domain entities. |

-------------------------
## [0.0.3] - 2026-04-09
### Domain Refactor: Catalog & Inventory Separation

| Layer | Details |
| :--- | :--- |
| **Core** | • Added structured hierarchy of custom exceptions.<br>• Separated Ingredient (physical item in Inventory) from IngredientDefinition (data in Catalog, i.e. `StorageLocation`). <br>• Entities refactor: replaced property setters with domain methods.<br>• Used natural keys for IngredientDefinitions and technical GUIDs for Ingredients.<br>• Domain: Entities, Enums (with validations), Exceptions. |
| **Application** | • Catalog Service for IngredientDefinitions. <br>• Introduced `IngredientCommands` for future CQRS implementation. |
| **API** | • Added controller for Catalog.<br>• Refactored Data Contracts: added requests for Catalog and Inventory.|

-------------------------
## [0.0.2] - 2026-04-07
### Single Responsibility Principle

| Layer | Details |
| :--- | :--- |
| **Application** | • Introduced the service pattern to pull business logic out of the controller. <br>• Used modern expression-bodied members for cleanup.|
| **API & Application** | Added support for CRUD operations (service and the controller). |
| **API**| Simplified the controller to focus strictly on handling HTTP requests. |

-------------------------
## [0.0.1] - 2026-04-03
### Basic Ingredients API

| Layer | Details |
| :--- | :--- |
| **API** | • Added basic GET/POST endpoints for ingredients. <br>• Added global enum serialization converter (JSON). |
| **Core** | Introduced the `UnitType` enum supporting Grams, Liters, and Pieces. |
| **DevOps** | Configured launch settings for automatic Swagger startup. |

-------------------------
## [0.0.0] - 2026-04-01
### Initial Commit.