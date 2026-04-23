# Release Notes - Kitchen.Api

[//]: # '[Version X] - YYYY-MM-DD'
[//]: # 'Added / Modified / Fixed / Deleted'


## [0.0.6] - 2026-04-23
### Added:
| Feature | Description |
| :--- | :--- |
| **Repository Pattern** | Introduced `IIngredientRepository` and `IIngredientTypeRepository` to abstract data access from business logic. |
| **In-Memory Persistence** | Implemented `InMemory` versions of repositories to centralize data management outside of services. |
| **Domain Exception Flow** | Added `IngredientNotFoundException` to handle missing resources via a unified exception-driven approach. |
| **Service Mocking** | Integrated `Moq` into the test suite to allow isolated unit testing of services by mocking repository dependencies. |

### Modified:
| Feature | Description |
| :--- | :--- |
| **Dependency Injection** | Refactored `Program.cs` to register repositories with a `Singleton` lifetime, ensuring data persistence across requests. |
| **Service Logic** | Updated `InventoryService` and `CatalogService` to delegate data operations to repositories and use exception-based validation. |
| **Controller Simplification** | Streamlined `Delete` and `Update` actions in controllers; results are now handled by the exception flow instead of boolean checks. |
| **Entity Renaming** | Renamed `IngredientDefinition` to `IngredientType` for better domain clarity and consistency with the Catalog nomenclature. |


## [0.0.5] - 2026-04-16
### Added:
| Feature | Description |
| :--- | :--- |
| **Unit Testing Project** | Created `Kitchen.Tests.Unit` using xUnit and FluentAssertions to ensure domain logic reliability. |
| **Command Pattern Refinement** | Introduced specific command sets: `InventoryCommands` (Stock management) and `CatalogCommands` (Definition management). |
| **Specialized Update DTOs** | Added `UpdateIngredientRequest` and `UpdateIngredientDefinitionRequest` with nullable support for partial updates (PATCH style). |
| **New Domain Entities Tests** | Implemented comprehensive test suites for `Ingredient` and `IngredientDefinition` state transitions. |

### Modified:
| Feature | Description |
| :--- | :--- |
| **Service Layer Renaming** | Renamed `IIngredientCatalogService` to `ICatalogService` for better brevity and architectural consistency. |
| **Nullable Domain Methods** | Refactored domain methods (`AdjustAmount`, `PlaceOrMove`, `ChangeUnitType`) to handle nullable inputs, enabling optional field updates. |
| **Controller Modernization** | Updated `IngredientsController` and `IngredientDefinitionsController` to process Commands instead of raw Entities or DTOs. |
| **API Responses** | Improved POST endpoints to return `201 Created` with proper `CreatedAtAction` location headers. |

### Fixed:
| Feature | Description |
| :--- | :--- |
| **Typo in Exceptions** | Corrected "Uknown" to "Unknown" in `UnknownLocationException`. |


## [0.0.4] - 2026-04-09
### Added:
| Feature | Description |
| :--- | :--- |
| **Command Pattern Base** | Introduced `IngredientCommands` to separate business intentions (Actions) from data structures. |
| **Domain Layer** | Established a dedicated `Domain` namespace for Entities, Enums, and Exceptions to isolate core business logic. |

### Modified:
| Feature | Description |
| :--- | :--- |
| **DTO Simplification** | Consolidated multiple Request objects into unified `IngredientDto` and `IngredientDefinitionDto`, reducing code redundancy. |
| **Namespace Refactor** | Reorganized project structure by moving domain-related files from `Models` to `Domain` for better architectural clarity. |
| **API Cleanup** | Removed obsolete `UpdateIngredientRequest` and `UpdateIngredientDefinitionRequest` files. |


## [0.0.3] - 2026-04-09
### Added:
| Feature | Description |
| :--- | :--- |
| **Ingredient Catalog** | Introduced `IngredientDefinition` to store global product metadata (name, default unit) separately from physical stock items. |
| **Storage Management** | Added `StorageLocation` enum (Fridge, Freezer, Pantry) to track exactly where items are kept in the kitchen. |
| **Custom Exception System** | Implemented a dedicated exception hierarchy starting from `KitchenApiException` (e.g., `IncorrectAmountException`, `UnknownLocationException`). |
| **Catalog API** | Added `IngredientDefinitionsController` to manage the global product dictionary and unit types. |

### Modified:
| Feature | Description |
| :--- | :--- |
| **DDD Domain Logic** | Refactored `Ingredient` methods to use domain-driven naming: `AdjustAmount` and `PlaceOrMove` instead of technical setters. |
| **Architecture Split** | Decoupled the service layer into `IngredientCatalogService` (catalog management) and `InventoryService` (stock management). |
| **Resource Identification** | Standardized using `Name` as a Natural Key for definitions and technical `Id` for physical inventory items. |
| **Data Transfer Objects** | Refactored DTOs to separate catalog concerns (Units) from inventory concerns (Location, Amount). |
| **Validation Logic** | Enhanced state protection using `Enum.IsDefined` to prevent `Unspecified` values from entering the domain. |


## [0.0.2] - 2026-04-07
### Modified:

| Feature | Description |
| :--- | :--- |
| **Service Layer Pattern** | Introduced IInventoryService and InventoryService to decouple business logic from the controller. |
| **Single Responsibility** | Refactored IngredientsController to focus solely on HTTP request handling (SRP). |
| **Full CRUD Support** | Expanded service and controller capabilities to include PUT (Update) and DELETE operations. |
| **Code Cleanup** | Applied expression-bodied members (=>) for improved readability across the service layer. |

## [0.0.1] - 2026-04-03
### Added:

| Feature | Description |
| :--- | :--- |
| **Ingredients Controller** | Implemented GET and POST endpoints for managing kitchen ingredients. |
| **Unit System** | Added UnitType enum supporting Grams, Liters, and Pieces. |
| **JSON Serialization** | Added JsonStringEnumConverter to allow sending and receiving Enum values as strings (e.g., "Liter") instead of integers. |
| **Launch Configuration** | Updated the application to start directly on the /swagger endpoint for better developer experience. |


## [0.0.0] - 2026-04-01
Initial Commit.