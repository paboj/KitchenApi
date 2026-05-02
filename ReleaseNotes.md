# Release Notes - Kitchen.Api

[//]: # '[Version X] - YYYY-MM-DD'
[//]: # 'Added / Modified / Fixed / Deleted'


-----------------------------------------------------------
## [1.0.0] - Release 1: "The Digital Pantry" - 2026-05-02
-----------------------------------------------------------

### Purpose & Business Value
The primary goal of this first release is to provide a digital tracking system for your home food storage. It allows users to maintain an accurate, up-to-date record of what is currently in their fridge and pantry, reducing the need to manually memorize ingredients and helping to prevent food waste.

---

### Features & Scope
The initial scope provides the complete foundational features for digital pantry management:

* **Inventory Management:** Supports adding, viewing, modifying, and deleting stored food items. Ingredients can be assigned to specific physical storage locations such as the fridge or pantry.
* **Product Catalog:** Implements a global dictionary of ingredient types to maintain naming and unit consistency (Grams, Liters, Pieces) across the application.

---

### Architecture & Technical Features
The application is built around the principles of Clean Architecture and Domain-Driven Design (DDD), ensuring high maintainability and long-term scalability.

#### 1. Layer Responsibilities
* **`Kitchen.Core` (Domain):** The heart of the application. Contains pure business logic, entities, and value objects (`IngredientId`, `IngredientName`), strictly protecting the application against invalid data states.
* **`Kitchen.Application` (Application):** Orchestration layer. Receives incoming API requests, transforms them into business intents (`Commands`), and executes logic using services.
* **`Kitchen.Infrastructure` (Infrastructure):** Technical implementation details. Handles database communication with PostgreSQL using Entity Framework Core.
* **`Kitchen.Api` (Presentation):** Exposes REST API endpoints and provides Swagger UI documentation for clients.

#### 2. Technical Foundation
* **Database:** PostgreSQL running in a Docker container via `docker-compose`.
* **Database Management:** Automated migrations and test data seeding on application startup.
* **Security & Configuration:** Uses the Options Pattern to securely bind and inject the database connection string.
* **Testing:** Dedicated testing project (xUnit + FluentAssertions) to validate the correctness of domain rules and application logic.

* 
-------------------------
## [0.0.9] - 2026-04-30
-------------------------

### Added
| Feature | Description |
| :--- | :--- |
| **Database** | Integrated **PostgreSQL** via EF Core as the primary database storage. |
| **Infrastructure** | Added `KitchenDbContext` with dedicated entity configurations for automated mapping. |
| **DevOps** | Introduced `docker-compose.yml` to spin up the local database environment quickly. |
| **EF Core Tools** | Implemented `IDesignTimeDbContextFactory` to support migrations in a multi-project setup. |
| **Data Seeding** | Added automated migrations and test data seeding on application startup. |

### Modified
| Feature | Description |
| :--- | :--- |
| **Repositories** | Migrated from in-memory collections to **PostgreSQL repositories** using LINQ. |
| **DI Lifetimes** | Changed service registrations from `Singleton` to `Scoped` for thread-safe DbContext usage. |
| **Domain Entities** | Added private constructors to domain entities to ensure EF Core compatibility. |
| **Startup Pipeline** | Moved migrations and seeding to a dedicated `IHostedService` to keep startup clean. |
| **Configuration** | Extracted the database connection string to `appsettings.json` using the **Options Pattern**. |

-------------------------
## [0.0.8] - 2026-04-25
-------------------------

### Modified
| Feature | Description |
| :--- | :--- |
| **Testing** | Added `InternalsVisibleTo` to give unit tests access to internal application and infrastructure classes. |
| **Validation** | Replaced manual null-checks with `Enum.IsDefined` for better type safety in domain models. |
| **DI Pattern** | Cleaned up Dependency Injection by standardizing registration classes as `Extensions.cs` in all projects. |
| **Project Structure** | Moved documentation files (`README.md`, `ReleaseNotes.md`) to the repository root for better visibility. |

-------------------------
## [0.0.7] - 2026-04-24
-------------------------

### Added
| Feature | Description |
| :--- | :--- |
| **Kitchen.Core** | Created domain layer containing core entities, value objects, and repository interfaces. |
| **Kitchen.Application** | Created application layer to handle business logic via services and commands. |
| **Kitchen.Infrastructure** | Created infrastructure layer for technical implementations like data access. |

### Modified
| Feature | Description |
| :--- | :--- |
| **Architecture** | Split the monolith into a multi-project solution to prevent leaky abstractions. |
| **Visibility** | Applied the `internal` modifier to implementations, exposing only interfaces to the outside. |
| **Dependency Flow** | Enforced a strict one-way dependency flow pointing directly towards the domain core. |

-------------------------
## [0.0.6] - 2026-04-23
-------------------------

### Added
| Feature | Description |
| :--- | :--- |
| **Repository Pattern** | Introduced repository contracts to decouple data access from core business logic. |
| **In-Memory Storage** | Created temporary in-memory repositories to centralize data management. |
| **Exceptions** | Added specialized domain exceptions like `IngredientNotFoundException` for unified error handling. |
| **Service Mocking** | Integrated `Moq` into the test suite to easily isolate service tests from real repositories. |

### Modified
| Feature | Description |
| :--- | :--- |
| **DI Lifetimes** | Switched repository registrations to `Singleton` to keep in-memory data alive between requests. |
| **Service Logic** | Updated services to delegate database operations to repositories and validate using exceptions. |
| **API Cleanup** | Simplified controllers by letting the exception flow handle error responses instead of boolean checks. |
| **Domain Entities** | Renamed `IngredientDefinition` to `IngredientType` for better clarity and alignment with the catalog. |

-------------------------
## [0.0.5] - 2026-04-16
-------------------------

### Added
| Feature | Description |
| :--- | :--- |
| **Testing** | Created the `Kitchen.Tests.Unit` project using xUnit and FluentAssertions for domain testing. |
| **Commands** | Refined the command pattern by grouping actions into `InventoryCommands` and `CatalogCommands`. |
| **Requests** | Introduced dedicated update requests with nullable types to easily support partial updates. |
| **Entity Tests** | Added comprehensive unit tests to verify state transitions in domain entities. |

### Modified
| Feature | Description |
| :--- | :--- |
| **Service Names** | Renamed the catalog service to `ICatalogService` for better brevity and readability. |
| **Domain Methods** | Updated domain methods to accept nullable types, making optional updates much simpler. |
| **Controllers** | Refactored endpoints to accept specific commands instead of passing raw entities. |
| **API Responses** | Improved POST endpoints to return `201 Created` with a valid location header. |

### Fixed
| Feature | Description |
| :--- | :--- |
| **Typos** | Fixed a spelling error in `UnknownLocationException`. |

-------------------------
## [0.0.4] - 2026-04-09
-------------------------

### Added
| Feature | Description |
| :--- | :--- |
| **Commands** | Introduced the Command pattern to separate business intent from pure data models. |
| **Domain Layer** | Extracted core files into a dedicated domain namespace to isolate business rules. |

### Modified
| Feature | Description |
| :--- | :--- |
| **DTOs** | Consolidated redundant request objects into unified, reusable DTO models. |
| **Namespaces** | Reorganized files from `Models` into the `Domain` namespace for better architectural clarity. |
| **API Cleanup** | Deleted old, obsolete request classes to keep the codebase clean. |

-------------------------
## [0.0.3] - 2026-04-09
-------------------------

### Added
| Feature | Description |
| :--- | :--- |
| **Ingredient Catalog** | Added `IngredientDefinition` to store global metadata separately from physical items. |
| **Storage Tracking** | Introduced the `StorageLocation` enum to manage where items are stored in the kitchen. |
| **Domain Exceptions** | Created a structured hierarchy of custom exceptions starting from a base API exception. |
| **Catalog API** | Created a new controller to manage global ingredient metadata and unit types. |

### Modified
| Feature | Description |
| :--- | :--- |
| **Domain Logic** | Refactored entities to use expressive domain methods instead of technical property setters. |
| **Architecture** | Separated the service layer into a catalog service and an inventory service. |
| **Resource Identifiers** | Used natural keys for global metadata and technical GUIDs for physical stock items. |
| **Data Contracts** | Split requests into dedicated models for catalog management and physical inventory management. |
| **Validation** | Protected domain invariants using `Enum.IsDefined` to block invalid enum values. |

-------------------------
## [0.0.2] - 2026-04-07
-------------------------

### Modified
| Feature | Description |
| :--- | :--- |
| **Service Layer** | Introduced the service pattern to pull business logic out of the controller. |
| **Single Responsibility**| Simplified the controller to focus strictly on handling HTTP requests. |
| **CRUD Support** | Added full support for update and delete operations in both the service and the controller. |
| **Readability** | Cleaned up the service layer using modern expression-bodied members. |

-------------------------
## [0.0.1] - 2026-04-03
-------------------------

### Added
| Feature | Description |
| :--- | :--- |
| **Ingredients API** | Added basic GET and POST endpoints for managing physical kitchen ingredients. |
| **Unit System** | Introduced the `UnitType` enum supporting Grams, Liters, and Pieces. |
| **Serialization** | Added a global converter to serialize and deserialize enums as readable strings. |
| **Developer UX** | Set up the launch settings to open Swagger automatically when starting the application. |

-------------------------
## [0.0.0] - 2026-04-01
-------------------------
Initial Commit.