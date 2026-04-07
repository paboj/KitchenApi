# Release Notes - Kitchen.Api

[//]: # '[Version X] - YYYY-MM-DD'
[//]: # 'Added / Modified / Fixed / Deleted'

# [1.0.2] - 2026-04-07
## Modified (Architectural Refactor):

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