# Release Notes - Kitchen.Api

[//]: # '[Version X] - YYYY-MM-DD'
[//]: # 'Added / Modified / Fixed / Deleted'

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