# 📡 API Documentation

Base URL: `http://localhost:5099/api`

Swagger UI available at: `http://localhost:5099/swagger` (`Development` environment only)

All requests and responses use **JSON** (`Content-Type: application/json`).

> **Note:** `name` fields (`StockItem.Name`, `ProductDefinition.Name`) are backed by the `ProductName` value object, which normalizes on construction — trims whitespace and **lowercases** the value. Send `"Mleko"` or `"MLEKO"`, get `"mleko"` back either way; this applies on every read and write.

---

## StockItems — Inventory

### GET `/api/stockitems`

Retrieves the list of all stock items (with the linked `ProductDefinition`, if one exists).

**Response `200 OK`:**

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "mleko",
    "amount": 2.5,
    "location": "lodówka",
    "definitionName": "mleko",
    "definition": {
      "name": "mleko",
      "unit": "l",
      "category": "nabiał"
    },
    "expirationDate": "2026-07-15"
  }
]
```

> **Note:** `unit`, `category`, and `location` come back as their Polish short form (`"l"`, `"nabiał"`, `"lodówka"`), not the C# enum name — see [Allowed enum values](#allowed-enum-values). If the item has no linked definition: `"definitionName": null, "definition": null`. If no expiration date is set: `"expirationDate": null`.

---

### GET `/api/stockitems/{id:guid}`

Retrieves a single item by `Id`.

**Responses:**

| Code | Description |
|---|---|
| `200 OK` | Found — returns a `StockItem` object (shape as above) |
| `404 Not Found` | No item with the given `Id` |

---

### GET `/api/stockitems/{name}`

Retrieves **all** items with the given name — `StockItem.Name` isn't unique (the same milk can sit in the fridge and in the pantry as two separate items).

**Path parameters:**

| Parameter | Type | Description |
|---|---|---|
| `name` | `string` | Item name |

**Responses:**

| Code | Description |
|---|---|
| `200 OK` | Returns an array of matching `StockItem`s |
| `404 Not Found` | No items with the given name |

---

### GET `/api/stockitems/expiring`

Retrieves all items whose `ExpirationDate` falls within the next `days` days (i.e. `ExpirationDate <= today + days`, including already-expired items).

**Query parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `days` | `int` | ❌ | Look-ahead window in days (default `7`) |

**Responses:**

| Code | Description |
|---|---|
| `200 OK` | Returns an array of matching `StockItem`s (empty array if none match) |

---

### POST `/api/stockitems`

Adds a new item to inventory.

**Request body:**

```json
{
  "name": "Mleko",
  "amount": 2.5,
  "location": "Fridge",
  "expirationDate": "2026-07-15"
}
```

`location` accepts either the English name (`"Fridge"`) or the Polish short form (`"lodówka"`), case-insensitively — see [Allowed enum values](#allowed-enum-values).

| Field | Type | Required | Description |
|---|---|---|---|
| `name` | `string` | ✅ | Item name (can't be blank or start with a digit) |
| `amount` | `double` | ✅ | Quantity (≥ 0) |
| `location` | `StorageLocation` | ❌ | Storage location (defaults to `Unspecified`) |
| `expirationDate` | `date` (`yyyy-MM-dd`) | ❌ | Expiration date; no "not in the past" validation — an expired item is still a real item |

**Responses:**

| Code | Description |
|---|---|
| `201 Created` | Item was added |
| `400 Bad Request` | Invalid input — including an unrecognized `location` value |

> **Note:** the `201` response body is the **created item itself**, including its generated `Id`, with a `Location` header pointing at `GET /api/stockitems/{id:guid}`.
>
> If a `ProductDefinition` with the same name exists, it's linked automatically (`InventoryService.Add`).

---

### PUT `/api/stockitems/{id:guid}`

Updates an existing item by `Id`. All fields are optional — send only what you want to change.

**Request body:**

```json
{
  "name": "Mleko 2%",
  "amount": 5.0,
  "location": "Pantry",
  "expirationDate": "2026-07-20"
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `name` | `string?` | ❌ | New name |
| `amount` | `double?` | ❌ | New quantity (≥ 0) |
| `location` | `StorageLocation?` | ❌ | New storage location |
| `expirationDate` | `date?` | ❌ | New expiration date |

**Responses:**

| Code | Description |
|---|---|
| `204 No Content` | Updated successfully |
| `400 Bad Request` | Invalid input |
| `404 Not Found` | No item with the given `Id` |

---

### DELETE `/api/stockitems/{id:guid}`

Removes an item from inventory.

**Responses:**

| Code | Description |
|---|---|
| `204 No Content` | Deleted successfully |
| `404 Not Found` | No item with the given `Id` |

---

## ProductDefinitions — Product catalog

### GET `/api/productdefinitions`

Retrieves the list of all product definitions.

**Response `200 OK`:**

```json
[
  {
    "name": "mleko",
    "unit": "l",
    "category": "nabiał"
  }
]
```

> `unit` and `category` come back as their Polish short form, same as in `StockItem` responses — see [Allowed enum values](#allowed-enum-values).

---

### GET `/api/productdefinitions/{name}`

Retrieves a single definition by name.

**Path parameters:**

| Parameter | Type | Description |
|---|---|---|
| `name` | `string` | Definition name |

**Responses:**

| Code | Description |
|---|---|
| `200 OK` | Found — returns a `ProductDefinition` object (shape as above) |
| `404 Not Found` | No definition with the given name |

---

### POST `/api/productdefinitions`

Creates a new product definition.

**Request body:**

```json
{
  "name": "Mleko",
  "unit": "Liters",
  "category": "Dairy"
}
```

`unit` and `category` each accept either the English name or the Polish short form, case-insensitively — see [Allowed enum values](#allowed-enum-values).

| Field | Type | Required | Description |
|---|---|---|---|
| `name` | `string` | ✅ | Unique product name (primary key) |
| `unit` | `UnitType` | ✅ | Unit of measure |
| `category` | `Category` | ✅ | Product category |

**Responses:**

| Code | Description |
|---|---|
| `201 Created` | Definition was added; body is the created definition itself |
| `400 Bad Request` | Invalid input — including an unrecognized `unit`/`category` value |
| `409 Conflict` | A definition with this name already exists |

> **Note:** if `StockItem`s with the same name already exist without a linked definition, `CatalogService.Add` links them automatically (`LinkToExistingStockItems`).

---

### PUT `/api/productdefinitions/{name}`

Updates an existing product definition.

**Request body:**

```json
{
  "unit": "Kilograms",
  "category": "DryGoods"
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `unit` | `UnitType?` | ❌ | New unit of measure |
| `category` | `Category?` | ❌ | New category |

**Responses:**

| Code | Description |
|---|---|
| `204 No Content` | Updated successfully |
| `400 Bad Request` | Invalid input |
| `404 Not Found` | No definition with the given name |

---

### DELETE `/api/productdefinitions/{name}`

Removes a product definition.

**Responses:**

| Code | Description |
|---|---|
| `204 No Content` | Deleted successfully |
| `404 Not Found` | No definition with the given name |

---

## Allowed enum values

`UnitType`, `Category`, and `StorageLocation` each have a dedicated `JsonConverter` (`Kitchen.Api/Serialization/`), registered globally in `Program.cs`. Each one accepts, case-insensitively: its English C# name, its Polish short form (the `[Description]` attribute on the enum member), or `"-"` / `"unspecified"` for the default value. Anything else is rejected with a `400 Bad Request` — see [Error format](#error-format).

On the way out, all three are written as their Polish short form, not the English enum name.

### UnitType

| JSON value (input) | Enum |
|---|---|
| `"-"`, `"unspecified"` | `Unspecified` |
| `"szt"`, `"sztuk"`, `"pieces"` | `Pieces` |
| `"kg"`, `"kilograms"` | `Kilograms` |
| `"l"`, `"liters"`, `"litry"` | `Liters` |

Output: `"-"`, `"szt"`, `"kg"`, `"l"`.

### Category

| JSON value (input) | Enum |
|---|---|
| `"-"`, `"unspecified"` | `Unspecified` |
| `"meat"`, `"mięso"` | `Meat` |
| `"vegetables"`, `"warzywa"` | `Vegetables` |
| `"dairy"`, `"nabiał"` | `Dairy` |
| `"drygoods"`, `"sypkie"` | `DryGoods` |
| `"spices"`, `"przyprawy"` | `Spices` |
| `"other"`, `"inne"` | `Other` |

Output: `"-"`, `"mięso"`, `"warzywa"`, `"nabiał"`, `"sypkie"`, `"przyprawy"`, `"inne"`.

### StorageLocation

| JSON value (input) | Enum |
|---|---|
| `"-"`, `"unspecified"` | `Unspecified` (0) |
| `"fridge"`, `"lodówka"` | `Fridge` (1) |
| `"freezer"`, `"zamrażarka"` | `Freezer` (2) |
| `"pantry"`, `"szafki"` | `Pantry` (3) |

Output: `"-"`, `"lodówka"`, `"zamrażarka"`, `"szafki"`.

> ⚠️ **Note:** this replaced an earlier version of `UnitTypeConverter` that silently fell back to `Unspecified` for any unrecognized value instead of rejecting it — a typo like `"kilo"` used to save silently as `Unspecified` instead of failing with a `400`. Fixed 2026-08-01.

---

## Error format

All errors return JSON in a uniform shape (`Kitchen.Infrastructure.Middleware.ExceptionMiddleware`):

```json
{
  "code": "product_definition_not_found",
  "message": "Human-readable error message"
}
```

`code` is the exception class name minus the `Exception` suffix, in `snake_case` (`Humanizer.Underscore()`).

| Exception | `code` | HTTP |
|---|---|---|
| `StockItemNotFoundException` | `stock_item_not_found` | `404 Not Found` |
| `ProductDefinitionNotFoundException` | `product_definition_not_found` | `404 Not Found` |
| `ProductDefinitionAlreadyExistsException` | `product_definition_already_exists` | `409 Conflict` |
| `InvalidProductNameException` | `invalid_product_name` | `400 Bad Request` |
| `IncorrectAmountException` | `incorrect_amount` | `400 Bad Request` |
| `UnknownLocationException` | `unknown_location` | `400 Bad Request` |
| `UnknownCategoryException` | `unknown_category` | `400 Bad Request` |
| `UnknownUnitTypeException` | `unknown_unit_type` | `400 Bad Request` |
| Other `KitchenApiException` | *(exception name)* | `400 Bad Request` |
| Unexpected errors | *(exception name or `exception`)* | `500 Internal Server Error` — logged via `ILogger<ExceptionMiddleware>` |
