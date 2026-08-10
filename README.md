# 🍽️ Kitchen.Api

A REST API for managing a kitchen pantry — tracks inventory (`StockItems`) and a catalog of product definitions (`ProductDefinitions`).

Built with **.NET 8** using **Clean Architecture**, a **PostgreSQL** database (Npgsql.EntityFrameworkCore.PostgreSQL), and **Entity Framework Core**.

---

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Requirements](#-requirements)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [API Endpoints](#-api-endpoints)
- [Data Model](#-data-model)
- [Error Handling](#-error-handling)
- [Project Structure](#-project-structure)
- [Testing](#-testing)

---

## ✅ Features

- **Inventory** management (`StockItems`): add, edit, and delete by `Id` (GUID), browse all items, and search by name
- **Product definition catalog** management (`ProductDefinitions`): ingredient types with a unit of measure and a category
- Automatic `StockItem ↔ ProductDefinition` linking by name — in both directions (when a stock item is added, if a matching definition exists; and when a definition is added, if unlinked stock items with the same name exist)
- Automatic database initialization (migrations + seed data) on application startup
- Global error handling with readable JSON error messages
- CORS configured for a local development frontend (`http://localhost:5173`)
- Swagger documentation available in development mode

---

## 🏗️ Architecture

The project follows **Clean Architecture**, split into four layers:

```
Kitchen.Api              ← Presentation layer (controllers, DI)
Kitchen.Application      ← Application logic (services, commands, request models)
Kitchen.Core             ← Domain (entities, value objects, exceptions, repository interfaces)
Kitchen.Infrastructure   ← Infrastructure (EF Core, PostgreSQL, repositories, migrations, exception middleware)
```

Dependencies only flow inward — `Infrastructure` and `Application` depend on `Core`, `Api` depends on `Application`.

A deeper walkthrough of each layer lives in [docs/architecture.md](docs/architecture.md), and the full endpoint reference (including the exact JSON shape) is in [docs/api.md](docs/api.md).

---

## 📦 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/) (version 13+)
- (optional) [Docker](https://www.docker.com/) to run the database

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd Kitchen.Api
```

### 2. Start the PostgreSQL database

A ready-made `docker-compose.yml` is included:

```bash
docker compose up -d
```

Or manually:

```bash
docker run --name kitchen-api-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=KitchenDb \
  -p 5432:5432 \
  -d postgres
```

### 3. Configure the connection string

`appsettings.json` ships with an empty `database:ConnectionString` on purpose — the real value lives in local user-secrets, not in a tracked file (avoids committing even a local-only credential):

```bash
dotnet user-secrets set "database:ConnectionString" "Host=localhost;Database=KitchenDb;Username=postgres;Password=postgres" --project Kitchen.Api
```

One-time setup per machine — picked up automatically in `Development`. `KitchenDbContextFactory` (used by the EF Core CLI) reads the same secret directly and throws a clear error if it isn't set. More commands: [docs/runbook.md](docs/runbook.md).

### 4. Run the application

```bash
dotnet run --project Kitchen.Api
```

Database migrations are **applied automatically** on application startup (`DatabaseInitBackgroundService`), and if the `StockItems` table is empty, sample seed data is added.

### 5. Open Swagger UI

```
http://localhost:5099/swagger
```

### Working with EF Core migrations

Design-time `DbContext` creation is handled by `KitchenDbContextFactory` (`IDesignTimeDbContextFactory<KitchenDbContext>`), so the EF Core CLI works without running the application:

```bash
dotnet ef migrations add <MigrationName> --project Kitchen.Infrastructure --startup-project Kitchen.Api
```

---

## ⚙️ Configuration

| Key | Description | Default value |
|---|---|---|
| `database:ConnectionString` | PostgreSQL connection string | *(empty in `appsettings.json` — set via `dotnet user-secrets`, see above)* |
| `Logging:LogLevel:Default` | Logging level | `Information` |

Detailed errors (`DetailedErrors: true`) are enabled in the `Development` environment.

---

## 📡 API Endpoints

Base URL: `http://localhost:5099/api`

### StockItems — inventory

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/stockitems` | Get all inventory items (with the linked `ProductDefinition`, if one exists) |
| `GET` | `/api/stockitems/{id:guid}` | Get an item by `Id` — `404` if it doesn't exist |
| `GET` | `/api/stockitems/{name}` | Get **all** items with the given name — `404` if there are no results |
| `GET` | `/api/stockitems/expiring?days={n}` | Get items expiring within `n` days (default `7`) |
| `POST` | `/api/stockitems` | Add a new item to inventory — `201 Created` |
| `PUT` | `/api/stockitems/{id:guid}` | Update an item by `Id` — `204 No Content` |
| `DELETE` | `/api/stockitems/{id:guid}` | Delete an item by `Id` — `204 No Content` |

> **Note:** `StockItem` has no unique key on name — the same name can occur multiple times (e.g. milk in the fridge and milk in the pantry as two separate items), so identifying by `Id` is the only way to unambiguously update/delete an item.

### ProductDefinitions — product catalog

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/productdefinitions` | Get all product definitions |
| `GET` | `/api/productdefinitions/{name}` | Get a definition by name — `404` if it doesn't exist |
| `POST` | `/api/productdefinitions` | Add a new product definition — `201 Created` |
| `PUT` | `/api/productdefinitions/{name}` | Update a product definition (key: name) — `204 No Content` |
| `DELETE` | `/api/productdefinitions/{name}` | Delete a product definition — `204 No Content` |

---

## 🗂️ Data Model

### Enums

| Enum | Values |
|---|---|
| `UnitType` | `Unspecified`, `Pieces` (szt), `Kilograms` (kg), `Liters` (l) |
| `Category` | `Unspecified`, `Meat` (mięso), `Vegetables` (warzywa), `Dairy` (nabiał), `DryGoods` (sypkie), `Spices` (przyprawy), `Other` (inne) |
| `StorageLocation` | `Unspecified`, `Fridge` (lodówka), `Freezer` (zamrażarka), `Pantry` (szafki) |

All three have a dedicated `JsonConverter` — each accepts its English name or Polish short form on input, and writes the Polish short form on output. See [docs/api.md](docs/api.md#allowed-enum-values) for the exact alias tables.

### Value Objects

- **`ProductName`** — product name; can't be empty or start with a digit; **normalized to lowercase** on construction (`ToLowerInvariant()`) — `"Mleko"` and `"MLEKO"` are the same name and both come back as `"mleko"`; has its own `JsonConverter`, so it serializes as a plain string
- **`StockItemId`** — a GUID identifying an inventory item; also has its own `JsonConverter`, serializing as a plain GUID string

---

## ⚠️ Error Handling

A global `ExceptionMiddleware` (`Kitchen.Infrastructure/Middleware`, `IMiddleware`) catches every exception, logs it, and returns a consistent JSON shape:

```json
{
  "code": "product_definition_not_found",
  "message": "Error message text"
}
```

`code` is the exception class name minus the `Exception` suffix, in `snake_case`.

| Exception | HTTP code |
|---|---|
| `StockItemNotFoundException` | `404 Not Found` |
| `ProductDefinitionNotFoundException` | `404 Not Found` |
| `ProductDefinitionAlreadyExistsException` | `409 Conflict` |
| `InvalidProductNameException` | `400 Bad Request` |
| `IncorrectAmountException` | `400 Bad Request` |
| `UnknownLocationException` | `400 Bad Request` |
| `UnknownCategoryException` | `400 Bad Request` |
| `UnknownUnitTypeException` | `400 Bad Request` |
| Other `KitchenApiException` | `400 Bad Request` |
| Unexpected errors | `500 Internal Server Error` (logged) |

Full specification, including the exact JSON shape for every endpoint: [docs/api.md](docs/api.md#error-format).

---

## 📁 Project Structure

```
Kitchen.Api/
├── Controllers/
│   ├── StockItemsController.cs         # StockItems endpoints (GET/POST/PUT/DELETE by Id, GET by name)
│   └── ProductDefinitionsController.cs # ProductDefinitions endpoints
├── Requests/
│   ├── CreateProductDefinitionRequest.cs
│   ├── CreateStockItemRequest.cs
│   ├── UpdateProductDefinitionRequest.cs
│   └── UpdateStockItemRequest.cs
├── Serialization/
│   ├── UnitTypeConverter.cs            # JsonConverter<UnitType>, PL aliases, registered globally
│   ├── CategoryConverter.cs            # JsonConverter<Category>, PL aliases
│   ├── StorageLocationConverter.cs     # JsonConverter<StorageLocation>, PL aliases
│   ├── ProductNameConverter.cs         # JsonConverter<ProductName> — plain string in/out
│   └── StockItemIdConverter.cs         # JsonConverter<StockItemId> — plain GUID string in/out
└── Program.cs

Kitchen.Application/
├── Commands/
│   ├── CatalogCommands.cs      # AddProductDefinitionCommand, ModifyProductDefinitionCommand
│   └── InventoryCommands.cs    # AddStockItemCommand, ModifyStockItemCommand
└── Services/
    ├── CatalogService.cs    # + LinkToExistingStockItems(), links new definitions to existing stock items
    └── InventoryService.cs  # uses the repository's *WithDetails variants

Kitchen.Core/
├── Domain/
│   ├── Entities/
│   │   ├── ProductDefinition.cs
│   │   └── StockItem.cs
│   ├── Enums/
│   │   ├── Category.cs
│   │   ├── StorageLocation.cs
│   │   └── UnitType.cs
│   └── Exceptions/
│       ├── KitchenApiException.cs   # base domain exception
│       ├── Catalog/    # ProductDefinitionNotFoundException, ProductDefinitionAlreadyExistsException
│       ├── Inventory/  # StockItemNotFoundException
│       └── Validation/ # InvalidProductNameException, IncorrectAmountException, UnknownLocationException, UnknownCategoryException, UnknownUnitTypeException
├── Repositories/
│   ├── IStockItemRepository.cs         # GetAll/GetById/GetByName/GetExpiring + WithDetails variants
│   └── IProductDefinitionRepository.cs
└── ValueObjects/
    ├── ProductName.cs
    └── StockItemId.cs

Kitchen.Infrastructure/
├── BackgroundServices/
│   └── DatabaseInitBackgroundService.cs  # migrations + seed data on startup
├── Middleware/
│   └── ExceptionMiddleware.cs   # IMiddleware, global error handling (see above)
└── DAL/
    ├── Configurations/
    │   ├── ProductDefinitionConfiguration.cs
    │   └── StockItemConfiguration.cs
    ├── Migrations/
    ├── Repositories/
    │   ├── PostgresStockItemRepository.cs
    │   └── PostgresProductDefinitionRepository.cs
    ├── KitchenDbContext.cs
    ├── KitchenDbContextFactory.cs   # IDesignTimeDbContextFactory — supports `dotnet ef`, hardcoded connection string
    ├── ConfigurationExtensions.cs   # GetOptions<T>() — generic appsettings binder
    └── PostgresOptions.cs
```

---

## 🧪 Testing

The project has two test layers: `Kitchen.Tests.Unit` (xUnit, FluentAssertions, Moq) and `Kitchen.Tests.Integration` (xUnit, FluentAssertions, **Testcontainers.PostgreSql** — spins up a real Postgres container and runs real migrations).

```bash
dotnet test Kitchen.Tests.Unit
dotnet test Kitchen.Tests.Integration   # requires Docker
```

CI (`.github/workflows/ci.yml`) runs both layers on PRs to `master` and pushes to `dev`.

Current unit test coverage:

| Layer / area | Test class | Scope |
|---|---|---|
| Domain | `StockItemTests` | constructor, `AdjustAmount`, `PlaceOrMove` |
| Domain | `ProductDefinitionTests` | constructor, `ChangeUnitType` |
| Application | `InventoryServiceTests` | `GetAll`, `GetByName`, `Add`, `Update`, `Delete` |
| Application | `CatalogServiceTests` | `Add` (including `LinkToExistingStockItems` and a regression test for `ProductName` equality) |
| Infrastructure | `ExceptionMiddlewareTests` | mapping exceptions to HTTP codes and JSON body |
| Api | `StockItemsControllerTests` | `GetAll`, `Get` (by id, by name), `Create`, `Update`, `Delete` |
| Api | `ProductDefinitionsControllerTests` | `GetAll`, `Get`, `Create`, `Update`, `Delete` |
| Api | `UnitTypeConverterTests`, `CategoryConverterTests`, `StorageLocationConverterTests` | alias mapping, unspecified handling, rejection of unrecognized input, description output |
| Api | `ProductNameConverterTests`, `StockItemIdConverterTests` | round-trip (de)serialization, invalid-input handling |

Integration tests (`Kitchen.Tests.Integration/Repositories/StockItemRepositoryTests`): auto-linking `StockItem` ↔ `ProductDefinition` against a real database, regression test for re-inserting an already-existing `ProductDefinition`.

Known coverage gaps: no unit tests yet for `SetName`/`AssignDefinition`/`SetExpirationDate` on `StockItem`.
