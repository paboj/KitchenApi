# Release Notes - Kitchen.Api

[//]: ## '[Version X] - YYYY-MM-DD'
[//]: ### 'Summary'

> Still `0.y.z` — API not yet stable, so no major bump is needed regardless of
> what changes. Real `1.0.0` lands once Digital Pantry closes out.

-------------------------
## [0.7.1] - 2026-07-09
### Expose GET by Name for ProductDefinitions

| Layer | Details |
| :--- | :--- |
| **Api** | • Added `GET /api/productdefinitions/{name}`, wrapping the `ICatalogService.GetByName()` that already existed in the application layer but wasn't reachable from the outside. |
| **Api** | • `POST /api/productdefinitions` now returns a `Location` header pointing at the new `Get` action instead of `GetAll` — a real self-link instead of pointing at the collection. |
| **Test** | • Updated `Create_ShouldReturnCreatedAtAction_WhenRequestIsValid` for the new action name; added `Get_ShouldReturnOk_WhenProductDefinitionExists` / `Get_ShouldReturnNotFound_WhenProductDefinitionDoesNotExist`. |

-------------------------
## [0.7.0] - 2026-07-08
### Fix JSON Enum Serialization

`StockItem.Location` switches wire format in `GET` responses from a raw integer to a string.

| Layer | Details |
| :--- | :--- |
| **Core** | • `StockItem.Location` now has `[JsonConverter(typeof(JsonStringEnumConverter))]` — was missing it, so `GET` responses returned it as a raw integer while `POST`/`PUT` requests took a string for the same field. |
| **Application** | • Removed the `[JsonConverter(typeof(JsonStringEnumConverter))]` attribute from `ProductDefinition.Unit` and `Create-`/`UpdateProductDefinitionRequest.Unit` — that attribute was shadowing the globally-registered `UnitTypeConverter` (property-level `[JsonConverter]` always wins over a converter in `JsonSerializerOptions.Converters`), so the Polish unit aliases (`szt`, `kg`, `l`, `litry`) never actually worked. |
| **Docs** | • `docs/api.md` updated to describe the corrected behavior — including a newly-noticed side effect: `UnitTypeConverter.Read` silently falls back to `Unspecified` for unrecognized input instead of throwing, so a typo in `unit` no longer produces a `400`. |

-------------------------
## [0.6.0] - 2026-07-06
### Expiration Dates & Definition Rename

`StockItem.Type`/`TypeName` renamed to `Definition`/`DefinitionName` — same rename on the wire, not just internally.

| Layer | Details |
| :--- | :--- |
| **Core** | • Added nullable `ExpirationDate` to `StockItem` (`SetExpirationDate`), intentionally without a "not in the past" validation — an expired item is still a real item.<br>• Renamed `StockItem.Type` → `StockItem.Definition` / `TypeName` → `DefinitionName` across the domain and EF configuration, to match `ProductDefinition` naming. |
| **Application** | • `AddStockItemCommand`/`ModifyStockItemCommand` now carry `ExpirationDate`.<br>• Fixed `CatalogService.LinkToExistingStockItems` re-inserting a `ProductDefinition` that a `StockItem` already referenced. |
| **Infrastructure** | • Added `RenameTypeToDefinition` migration.<br>• `ExceptionMiddleware` now logs unhandled exceptions before returning `500`. |
| **Test** | • Added integration coverage for `ProductDefinition` ↔ `StockItem` auto-linking.<br>• Regression test for a bug where `ProductName` compared by reference instead of value, silently breaking auto-linking.<br>• Fixed a missing logger mock in `ExceptionMiddlewareTests`. |
| **DevOps** | • Added `.gitattributes` to normalize line endings. |

-------------------------
## [0.5.1] - 2026-07-05
### Integration Testing & CI

| Layer | Details |
| :--- | :--- |
| **Infrastructure** | • Squashed all EF Core migrations into a single `Initial` baseline. |
| **Test** | • Added `Kitchen.Tests.Integration` — repository tests against a real PostgreSQL container via **Testcontainers**, running actual migrations instead of `EnsureCreated()`. |
| **DevOps** | • Added a GitHub Actions workflow: build + unit tests + integration tests on PRs to `master` and pushes to `dev`.<br>• Removed the gitignored `Private` project from the solution file.<br>• Dev environment fixes and a more realistic seed dataset. |

-------------------------
## [0.5.0] - 2026-07-04
### Exception Middleware Rewrite & CORS

Error response body reshaped from `{error,code,type}` to `{code,message}`.

| Layer | Details |
| :--- | :--- |
| **Infrastructure** | • Rewrote the global exception handler as `ExceptionMiddleware : IMiddleware`, moved from `Kitchen.Api` to `Kitchen.Infrastructure`.<br>• JSON error body changed to `{ "code", "message" }`, with `code` derived from the exception type name. |
| **Core** | • Reorganized exception files; added dedicated handling for `UnknownCategoryException`. |
| **Api** | • Added a CORS policy (`FrontendCorsPolicy`) allowing `http://localhost:5173` for local frontend development. |

-------------------------
## [0.4.0] - 2026-06-14
### GUID Identity & Async Everywhere

`StockItem` routing switches from name-based to GUID-based — old URLs stop resolving.

| Layer | Details |
| :--- | :--- |
| **Core** | • Switched `StockItem`'s identity from name-based to a GUID `StockItemId`.<br>• Fixed assorted runtime bugs; tightened encapsulation. |
| **All** | • Converted the full call chain — controllers, services, repositories — to `async`/`await`. |
| **Test** | • Updated unit tests for the async refactor. |

-------------------------
## [0.3.0] - 2026-05-13
### Domain Rename: Ingredient → StockItem / ProductDefinition

Routes move from `/api/ingredients` / `/api/ingredienttypes` to `/api/stockitems` / `/api/productdefinitions`.

| Layer | Details |
| :--- | :--- |
| **Core** | • Renamed `Ingredient` → `StockItem` and `IngredientType` → `ProductDefinition` across the codebase to better reflect the domain language. |
| **Application** | • `StockItem`s eagerly load their linked `ProductDefinition` via `*WithDetails` repository methods. |

-------------------------
## [0.2.0] - 2026-05-07
### Categories, Polish Unit Aliases & Stronger Validation

`Category` becomes a required field on `ProductDefinition` — old create requests without it now fail.

| Layer | Details |
| :--- | :--- |
| **Core** | • Added the `Category` enum (`Meat`, `Vegetables`, `Dairy`, `DryGoods`, `Spices`, `Other`) on `ProductDefinition`.<br>• `UnitTypeConverter` accepts Polish aliases (`szt`, `sztuk`, `kg`, `l`, `litry`, ...) on input. |
| **Application/Api** | • Fixed a bug in the update flow; added structured domain exception handling. |
| **Test** | • Expanded validation and service test coverage. |

-------------------------
## [0.1.0] - 2026-05-02
### Release 1: "The Digital Pantry"

The first genuinely usable version of the system — full CRUD tracking of
household food stock across storage locations, plus a product catalog.
Not yet `1.0.0`: as the versions above show, the contract kept moving
afterward, so it hadn't actually earned a stability promise yet.

#### Purpose & Business Value
Deliver a reliable tracking system for household food supplies. By
maintaining an accurate, real-time record of ingredients across various
storage locations, the system eliminates the need for manual tracking and
contributes to the reduction of food waste.

#### Features & Scope
* **Inventory Management:** Full CRUD (Create, Read, Update, Delete) capabilities for food items, with support for specific physical assignments: Fridge, Freezer, and Pantry.
* **Product Catalog:** A standardized dictionary of ingredient types to ensure data consistency and provide metadata for all products.

#### Architecture & Technical Design
Implemented following **Clean Architecture** and **Domain-Driven Design
(DDD)** principles:
* **Kitchen.Core (Domain):** Pure business logic, entities, and value objects; enforces business rules directly within the models.
* **Kitchen.Application:** Orchestration layer translating user intents into business actions (Commands).
* **Kitchen.Infrastructure:** PostgreSQL communication via Entity Framework Core.
* **Kitchen.Api (Presentation):** RESTful endpoints with integrated OpenAPI (Swagger) documentation.

Technical foundation: PostgreSQL via `docker-compose.yml` for local/production parity, automated migrations and data seeding on startup, externalized configuration via `appsettings.json`, and a dedicated xUnit/FluentAssertions test suite covering both domain rules and application orchestration.

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
