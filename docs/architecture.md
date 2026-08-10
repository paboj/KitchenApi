# 🏗️ Kitchen.Api Architecture

## Clean Architecture

The project is built on the **Clean Architecture** pattern, which separates responsibilities and keeps the domain layer independent of technical details (database, web framework, etc.).

```
┌─────────────────────────────────┐
│         Kitchen.Api             │  ← Presentation layer
│ (controllers, request models,   │
│  DI, Program.cs)                │
└────────────────┬────────────────┘
                 │ depends on
┌────────────────▼────────────────┐
│      Kitchen.Application        │  ← Application logic
│      (services, commands)       │
└────────────────┬────────────────┘
                 │ depends on
┌────────────────▼────────────────┐
│         Kitchen.Core            │  ← Domain (the center)
│ (entities, exceptions,          │
│  interfaces, value objects,     │
│  enums)                         │
└─────────────────────────────────┘
         ▲
         │ implements interfaces from Core
┌────────┴────────────────────────┐
│     Kitchen.Infrastructure      │  ← Infrastructure
│  (EF Core, PostgreSQL, repos,   │
│   exception middleware)         │
└─────────────────────────────────┘
```

> **Note:** the global `ExceptionMiddleware` (mapping domain exceptions to HTTP codes) lives in `Kitchen.Infrastructure`, not `Kitchen.Api`. That's a deliberate architectural choice — exception handling is a cross-cutting concern (applies to every request, regardless of layer), so it sits alongside the rest of the cross-cutting infrastructure rather than in the presentation layer. `Kitchen.Api` registers it via `app.UseInfrastructure()`.

---

## Layers

### Kitchen.Core — Domain

The center of the whole system. **Depends on no other layer in the project.**

#### Entities

**`ProductDefinition`** — the definition/type of a food product (e.g. "Milk", "Flour").

| Property | Type | Description |
|---|---|---|
| `Name` | `ProductName` | Primary key — product name |
| `Unit` | `UnitType` | Unit of measure (kg, l, pcs) |
| `Category` | `Category` | Product category |

Domain methods:
- `SetName(string)` — **private**, called only from the constructor (delegates validation to `ProductName`); there's no public way to rename a definition after creation
- `ChangeUnitType(UnitType?)` — changes the unit; throws `UnknownUnitTypeException` for an invalid value
- `SetCategory(Category?)` — sets the category; throws `UnknownCategoryException`

---

**`StockItem`** — a concrete item in kitchen inventory.

| Property | Type | Description |
|---|---|---|
| `Id` | `StockItemId` | Primary key (GUID) |
| `Name` | `ProductName` | Item name — **not unique** (see below) |
| `Amount` | `double` | Quantity (must be ≥ 0) |
| `Location` | `StorageLocation` | Storage location |
| `DefinitionName` | `ProductName?` | Name of the linked definition (shadow FK in EF) |
| `Definition` | `ProductDefinition?` | Optional link to a product definition |
| `ExpirationDate` | `DateOnly?` | Expiration date — no "not in the past" validation |

Domain methods:
- `SetName(string?)` — changes the name
- `AdjustAmount(double?)` — sets the quantity; throws `IncorrectAmountException` when < 0
- `PlaceOrMove(StorageLocation?)` — sets the location; throws `UnknownLocationException`
- `AssignDefinition(ProductDefinition?)` — assigns a product definition
- `SetExpirationDate(DateOnly?)` — sets the expiration date

`Name` has no unique key — the same name can occur multiple times (e.g. milk in the fridge and milk in the pantry as two separate items). Unambiguous identification is only possible via `Id`.

#### Value Objects

**`ProductName`** — wraps a string with validation:
- can't be empty or made up of only whitespace
- can't start with a digit
- automatically trims leading/trailing whitespace (`.Trim()`) and **normalizes to lowercase** (`.ToLowerInvariant()`) — `"Mleko"` and `"MLEKO"` are the same name, both stored/returned as `"mleko"`
- has implicit `string ↔ ProductName` conversions
- **value equality** (`IEquatable<ProductName>`, compares `Value` via `StringComparison.Ordinal`) — matters for `CatalogService.LinkToExistingStockItems`, which compares `ProductName` by name, not by reference
- has its own `JsonConverter` (`Kitchen.Api/Serialization/ProductNameConverter.cs`), so it serializes as a plain string, not `{ "value": "..." }`

**`StockItemId`** — `record StockItemId(Guid Value)`, `StockItem`'s primary key. Also has its own `JsonConverter` (`StockItemIdConverter.cs`), serializing as a plain GUID string.

#### Enums

| Enum | Values |
|---|---|
| `UnitType` | `Unspecified(0)`, `Pieces(1)`, `Kilograms(2)`, `Liters(3)` |
| `Category` | `Unspecified(0)`, `Meat(1)`, `Vegetables(2)`, `Dairy(3)`, `DryGoods(4)`, `Spices(5)`, `Other(6)` |
| `StorageLocation` | `Unspecified(0)`, `Fridge(1)`, `Freezer(2)`, `Pantry(3)` |

Every value has a `[Description]` attribute (e.g. `"kg"`, `"szt"`, `"mięso"`, `"lodówka"`), returned by `EnumExtensions.ToDescription()`. All three enums now have a dedicated `JsonConverter` that uses this same description for JSON output and accepts it as an alternate input alongside the English name — exact details of what actually goes over the wire in [docs/api.md](./api.md#allowed-enum-values).

#### Domain Exceptions

All inherit from `KitchenApiException : Exception`.

| Exception | Meaning |
|---|---|
| `StockItemNotFoundException` | The requested item doesn't exist |
| `ProductDefinitionNotFoundException` | The requested definition doesn't exist |
| `ProductDefinitionAlreadyExistsException` | A definition with this name already exists |
| `InvalidProductNameException` | Invalid product name |
| `IncorrectAmountException` | Negative quantity |
| `UnknownLocationException` | Unrecognized storage location |
| `UnknownUnitTypeException` | Unrecognized unit of measure |
| `UnknownCategoryException` | Unrecognized category |

Grouped into subfolders by concern: `Catalog/` (`ProductDefinitionNotFoundException`, `ProductDefinitionAlreadyExistsException`), `Inventory/` (`StockItemNotFoundException`), `Validation/` (the rest), plus the base `KitchenApiException` directly under `Exceptions/`.

#### Repository Interfaces

**`IStockItemRepository`:** `GetAll`, `GetById`, `GetByName`, `GetExpiring(DateOnly threshold)`, `Add`, `Update`, `Delete`, plus `GetAllWithDetails`/`GetByIdWithDetails`/`GetByNameWithDetails` variants (eager-load `Definition`).

**`IProductDefinitionRepository`:** `GetAll`, `GetByName`, `Add`, `Update`, `Delete`.

Defined in Core, implemented in Infrastructure — so the domain never knows any database details.

---

### Kitchen.Application — Application logic

Orchestrates the flow of data between the API and the domain. Contains no domain logic and no infrastructure details.

#### Services

**`ICatalogService` / `CatalogService`** — manages the product definition catalog:
- `GetAll()` — returns all definitions
- `GetByName(string)` — looks up a definition by name
- `Add(AddProductDefinitionCommand)` — checks the name is unique (`ProductDefinitionAlreadyExistsException`), creates the definition, then calls `LinkToExistingStockItems`; returns the created `ProductDefinition`
- `Update(ModifyProductDefinitionCommand)` — looks up the definition (`ProductDefinitionNotFoundException` if missing), updates unit/category
- `Delete(string)` — looks up the definition, deletes it
- `LinkToExistingStockItems(ProductDefinition)` — walks every `StockItem` and links this definition to any that share its name and don't already have one

**`IInventoryService` / `InventoryService`** — manages inventory:
- `GetAll()` / `GetById(Guid)` / `GetByName(string)` — use the repository's `*WithDetails` variants (eager-load `Definition`)
- `GetExpiring(int days)` — converts `days` into a `DateOnly` threshold (`UtcNow.AddDays(days)`) and delegates to `IStockItemRepository.GetExpiring`
- `Add(AddStockItemCommand)` — looks up a `ProductDefinition` by name and links it automatically if one exists; returns the created `StockItem`
- `Update(ModifyStockItemCommand)` — looks up the item by `Id` (`StockItemNotFoundException` if missing), updates name/amount/location/expiration date
- `Delete(Guid)` — looks up the item, deletes it

#### Commands (CQRS-like)

C# records carrying data from the controller to the service:

| Command | Fields |
|---|---|
| `AddProductDefinitionCommand` | `Name`, `Unit`, `Category` |
| `ModifyProductDefinitionCommand` | `Name`, `Unit?`, `Category?` |
| `AddStockItemCommand` | `Name`, `Amount`, `Location`, `ExpirationDate = null` |
| `ModifyStockItemCommand` | `Id`, `Name?`, `Amount?`, `Location?`, `ExpirationDate = null` |

---

### Kitchen.Infrastructure — Infrastructure

Implements data access and global exception handling. Depends on `Core` (implements its interfaces).

#### KitchenDbContext

The Entity Framework Core `DbContext`. Contains two `DbSet`s:
- `StockItems`
- `ProductDefinitions`

Entity configurations are loaded automatically from the assembly (`ApplyConfigurationsFromAssembly`).

#### EF Core Configurations

**`ProductDefinitionConfiguration`:**
- Primary key: `Name` (`ProductName ↔ string` conversion)
- `Unit` and `Category` stored as `int`

**`StockItemConfiguration`:**
- Primary key: `Id` (`StockItemId ↔ Guid` conversion)
- `Name` required, `ProductName ↔ string` conversion — **no unique index** (deliberate, `Name` isn't unique)
- `ExpirationDate` optional
- `Location` stored as `int`
- Shadow property `DefinitionName` (`ProductName ↔ string`, optional) + FK relationship: `DefinitionName → ProductDefinition.Name` (optional, `WithMany()` — one definition can have many linked items)

#### Repositories

| Class | Interface |
|---|---|
| `PostgresStockItemRepository` | `IStockItemRepository` |
| `PostgresProductDefinitionRepository` | `IProductDefinitionRepository` |

Both use `AsNoTracking()` on reads. `PostgresStockItemRepository` additionally offers `*WithDetails` variants with `Include(i => i.Definition)`.

`Add`/`Update` in `PostgresStockItemRepository` check whether `stockItem.Definition` is `Detached`, and call `Attach()` if so — `Definition` typically comes from an `AsNoTracking()` read, so `DbContext` isn't tracking it.

#### KitchenDbContextFactory

`IDesignTimeDbContextFactory<KitchenDbContext>` — lets the EF Core CLI (`dotnet ef migrations add ...`) create a `DbContext` without running the app. Since there's no DI container at design time, it reads `database:ConnectionString` itself via a standalone `ConfigurationBuilder` (`AddUserSecrets` with the same `UserSecretsId` as `Kitchen.Api.csproj`, plus environment variables), throwing a clear `InvalidOperationException` if the secret isn't set. No connection string is hardcoded anymore.

#### DatabaseInitBackgroundService

An `IHostedService` that runs at application startup:
1. Applies all pending EF Core migrations (`Database.MigrateAsync()`)
2. If the `StockItems` table is empty — seeds sample data (6 `ProductDefinition`s + 6 `StockItem`s, Polish names: Mleko, Jajka, Kurczak, Marchew, Ryż, Papryka mielona)

#### ExceptionMiddleware

`internal sealed class ExceptionMiddleware : IMiddleware` (namespace `Kitchen.Infrastructure.Middleware`) — the global exception handler. Logs every caught exception (`ILogger<ExceptionMiddleware>`), maps the exception type to an HTTP code, and returns `{ "code", "message" }` (code in `snake_case`, via `Humanizer.Underscore()`). Full mapping table: [docs/api.md — Error format](./api.md#error-format).

Registration: `AddTransient<ExceptionMiddleware>()` in `AddInfrastructure()`, usage: `app.UseMiddleware<ExceptionMiddleware>()` in `UseInfrastructure()` — both in `Kitchen.Infrastructure/InfrastructureExtensions.cs`.

#### Migrations

| Migration | Description |
|---|---|
| `Initial` (2026-07-05) | Starting point after squashing all earlier migrations into one |
| `AddExpirationDateToStockItem` (2026-07-05) | `ExpirationDate` column on `StockItems` |
| `RenameTypeToDefinition` (2026-07-06) | Renamed the `TypeName` column/FK → `DefinitionName` |

---

### Kitchen.Api — Presentation layer

#### Request Models

DTOs accepted from the HTTP request body. Controllers map each one directly into the matching `Command` before calling a service. `Name` here is a plain `string`, same as it now serializes in responses too, since `ProductName` has its own converter:

| Model | Fields |
|---|---|
| `CreateProductDefinitionRequest` | `Name`, `Unit`, `Category` |
| `UpdateProductDefinitionRequest` | `Unit?`, `Category?` |
| `CreateStockItemRequest` | `Name`, `Amount`, `Location`, `ExpirationDate?` |
| `UpdateStockItemRequest` | `Name?`, `Amount?`, `Location?`, `ExpirationDate?` |

#### Controllers

**`StockItemsController`** (`/api/stockitems`):

| Method | Route | Behavior |
|---|---|---|
| GET | `/` | `_inventoryService.GetAll()` |
| GET | `/{id:guid}` | `_inventoryService.GetById(id)` or 404 |
| GET | `/{name}` | `_inventoryService.GetByName(name)` or 404 (if the collection is empty) |
| GET | `/expiring?days={n}` | `_inventoryService.GetExpiring(days)`, default `days = 7`; always `200` (empty array if none match) |
| POST | `/` | Builds an `AddStockItemCommand`, calls `Add()`, returns 201 with the created item and a `Location` header pointing at `GET /{id:guid}` |
| PUT | `/{id:guid}` | Builds a `ModifyStockItemCommand`, calls `Update()`, returns 204 |
| DELETE | `/{id:guid}` | Calls `Delete()`, returns 204 |

**`ProductDefinitionsController`** (`/api/productdefinitions`):

| Method | Route | Behavior |
|---|---|---|
| GET | `/` | `_catalogService.GetAll()` |
| GET | `/{name}` | `_catalogService.GetByName(name)` or 404 |
| POST | `/` | Builds an `AddProductDefinitionCommand`, calls `Add()`, returns 201 with the created definition and a `Location` header pointing at `Get` |
| PUT | `/{name}` | Builds a `ModifyProductDefinitionCommand`, calls `Update()`, returns 204 |
| DELETE | `/{name}` | Calls `Delete()`, returns 204 |

#### Serialization/

Five `JsonConverter`s, all registered globally in `Program.cs`'s `AddJsonOptions`:

| Converter | Type | Behavior |
|---|---|---|
| `UnitTypeConverter` | `UnitType` | Accepts the English name, a Polish alias, or `-`/`unspecified`; outputs the `[Description]` short form; rejects anything else with `UnknownUnitTypeException` |
| `CategoryConverter` | `Category` | Same pattern as `UnitTypeConverter` |
| `StorageLocationConverter` | `StorageLocation` | Same pattern as `UnitTypeConverter` |
| `ProductNameConverter` | `ProductName` | Plain string in, plain string out — wraps/unwraps the value object |
| `StockItemIdConverter` | `StockItemId` | Plain GUID string in, plain GUID string out |

Full alias tables and exact JSON shapes: [docs/api.md](./api.md#allowed-enum-values).

#### Program.cs — registration order

```
AddControllers().AddJsonOptions(+ UnitTypeConverter, ProductNameConverter,
                                   StockItemIdConverter, CategoryConverter,
                                   StorageLocationConverter, relaxed JSON encoder)
AddCore() / AddApplication() / AddInfrastructure(config)
AddSwaggerGen()
AddCors("FrontendCorsPolicy" → http://localhost:5173)
─────────────
UseInfrastructure()   // registers ExceptionMiddleware
Swagger (Development only)
UseCors("FrontendCorsPolicy")
MapControllers()
```

---

## Dependency Registration

Each layer provides an `IServiceCollection` extension method:

| Method | Layer | Registers |
|---|---|---|
| `AddApplication()` | Application | `ICatalogService`, `IInventoryService` (Scoped) |
| `AddInfrastructure(config)` | Infrastructure | `KitchenDbContext` + repositories (Scoped), `DatabaseInitBackgroundService` (Hosted), `ExceptionMiddleware` (Transient) |
