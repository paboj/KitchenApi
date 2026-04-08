# Release Notes - Kitchen.Api

[//]: # '[Version X] - YYYY-MM-DD'
[//]: # 'Added / Modified / Fixed / Deleted'

## [1.0.3] - 2026-04-09
### Added:
| Feature | Description |
| :--- | :--- |
| **Data Transfer Objects** | Introduced `CreateIngredientRequest` and `UpdateIngredientRequest` to separate API contracts from internal domain models. |

### Modified:
| Feature | Description |
| :--- | :--- |
| **Domain Encapsulation** | Refactored `Ingredient` entity with private setters and domain methods (`SetAmount`, `ChangeUnitType`) to ensure data integrity and validation. |
| **Resource Identification** | Switched from `Guid` to `Name` as the primary identifier for `PUT` and `DELETE` operations to simplify API usage. |
| **Input Validation** | Added range checks for amounts and unit types to prevent invalid state in the kitchen inventory. |
| **Enhanced Enums** | Updated `UnitType` with an explicit `Unspecified` value to improve default state handling. |
| **Service Layer Refactor** | Updated `InventoryService` to support name-based logic and protect the internal state of ingredients. |

## [1.0.2] - 2026-04-07
### Modified:

| Feature | Description |
| :--- | :--- |
| **Service Layer Pattern** | Introduced IInventoryService and InventoryService to decouple business logic from the controller. |
| **Single Responsibility** | Refactored IngredientsController to focus solely on HTTP request handling (SRP). |
| **Full CRUD Support** | Expanded service and controller capabilities to include PUT (Update) and DELETE operations. |
| **Code Cleanup** | Applied expression-bodied members (=>) for improved readability across the service layer. |

## [1.0.1] - 2026-04-03
### Added:

| Feature | Description |
| :--- | :--- |
| **Ingredients Controller** | Implemented GET and POST endpoints for managing kitchen ingredients. |
| **Unit System** | Added UnitType enum supporting Grams, Liters, and Pieces. |
| **JSON Serialization** | Added JsonStringEnumConverter to allow sending and receiving Enum values as strings (e.g., "Liter") instead of integers. |
| **Launch Configuration** | Updated the application to start directly on the /swagger endpoint for better developer experience. |


## [1.0.0] - 2026-04-01
Initial Commit.