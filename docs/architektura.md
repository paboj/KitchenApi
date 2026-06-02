# 🏗️ Architektura Kitchen.Api

## Clean Architecture

Projekt oparty jest na wzorcu **Clean Architecture**, który zapewnia separację odpowiedzialności i niezależność warstw domenowych od szczegółów technicznych (bazy danych, frameworku webowego itp.).

```
┌─────────────────────────────────┐
│         Kitchen.Api             │  ← Warstwa prezentacji
│  (kontrolery, middleware, DI)   │
└────────────────┬────────────────┘
                 │ zależy od
┌────────────────▼────────────────┐
│      Kitchen.Application        │  ← Logika aplikacji
│   (serwisy, komendy, modele)    │
└────────────────┬────────────────┘
                 │ zależy od
┌────────────────▼────────────────┐
│         Kitchen.Core            │  ← Domena (centrum)
│ (encje, wyjątki, interfejsy,    │
│  value objects, enumy)          │
└─────────────────────────────────┘
         ▲
         │ implementuje interfejsy z Core
┌────────┴────────────────────────┐
│     Kitchen.Infrastructure      │  ← Infrastruktura
│  (EF Core, PostgreSQL, repos)   │
└─────────────────────────────────┘
```

---

## Warstwy

### Kitchen.Core — Domena

Centrum całego systemu. **Nie zależy od żadnej innej warstwy projektu.**

#### Encje

**`ProductDefinition`** — definicja/typ produktu spożywczego (np. „Mleko", „Mąka").

| Właściwość | Typ | Opis |
|---|---|---|
| `Name` | `ProductName` | Klucz główny — nazwa produktu |
| `Unit` | `UnitType` | Jednostka miary (kg, l, szt) |
| `Category` | `Category` | Kategoria produktu |

Metody domenowe:
- `ChangeUnitType(UnitType?)` — zmienia jednostkę; rzuca `UnknownUnitTypeException` przy nieprawidłowej wartości
- `SetCategory(Category?)` — ustawia kategorię; rzuca `UnknownCategoryException`

---

**`StockItem`** — konkretna pozycja w zapasach kuchennych.

| Właściwość | Typ | Opis |
|---|---|---|
| `Id` | `StockItemId` | Klucz główny (GUID) |
| `Name` | `ProductName` | Nazwa pozycji |
| `Amount` | `double` | Ilość (musi być ≥ 0) |
| `Location` | `StorageLocation` | Miejsce przechowywania |
| `Type` | `ProductDefinition?` | Opcjonalne powiązanie z definicją produktu |

Metody domenowe:
- `AdjustAmount(double?)` — ustawia ilość; rzuca `IncorrectAmountException` gdy < 0
- `PlaceOrMove(StorageLocation?)` — ustawia lokalizację; rzuca `UnknownLocationException`
- `AssignType(ProductDefinition?)` — przypisuje typ produktu

#### Value Objects

**`ProductName`** — opakowuje string z walidacją:
- nie może być pusty ani składać się z samych białych znaków
- nie może zaczynać się od cyfry
- automatycznie usuwa białe znaki z początku i końca (`.Trim()`)
- posiada niejawne konwersje `string ↔ ProductName`

**`StockItemId`** — opakowanie na `Guid`, klucz główny `StockItem`.

#### Enumy

| Enum | Wartości |
|---|---|
| `UnitType` | `Unspecified(0)`, `Pieces(1)`, `Kilograms(2)`, `Liters(3)` |
| `Category` | `Unspecified` + kategorie produktów |
| `StorageLocation` | `Unspecified`, `Fridge`, `Freezer`, `Pantry` |

Każda wartość `UnitType` posiada atrybut `[Description]` (np. `"kg"`, `"szt"`), zwracany przez `EnumExtensions.ToDescription()`.

#### Wyjątki domenowe

Wszystkie dziedziczą po `KitchenApiException : Exception`.

| Wyjątek | Znaczenie |
|---|---|
| `StockItemNotFoundException` | Szukana pozycja nie istnieje |
| `ProductDefinitionAlreadyExistsException` | Definicja o tej nazwie już istnieje |
| `InvalidProductNameException` | Nieprawidłowa nazwa produktu |
| `IncorrectAmountException` | Ujemna ilość |
| `UnknownLocationException` | Nieznana lokalizacja |
| `UnknownUnitTypeException` | Nieznana jednostka miary |
| `UnknownCategoryException` | Nieznana kategoria |

#### Interfejsy repozytoriów

`IStockItemRepository` i `IProductDefinitionRepository` — zdefiniowane w Core, implementowane w Infrastructure. Dzięki temu domena nie zna szczegółów bazy danych.

---

### Kitchen.Application — Logika aplikacji

Orkiestruje przepływ danych między API a domeną. Nie zawiera logiki domenowej ani szczegółów infrastrukturalnych.

#### Serwisy

**`ICatalogService` / `CatalogService`** — zarządzanie katalogiem definicji produktów:
- `GetAll()` — zwraca wszystkie definicje
- `GetByName(string)` — szuka definicji po nazwie
- `Add(AddTypeCatalogCommand)` — tworzy nową definicję; sprawdza unikalność nazwy
- `Update(ModifyTypeCatalogCommand)` — aktualizuje jednostkę/kategorię
- `Delete(string)` — usuwa definicję

**`IInventoryService` / `InventoryService`** — zarządzanie zapasami:
- `GetAll()` — zwraca wszystkie pozycje
- `GetByName(string)` — szuka pozycji po nazwie
- `Add(AddToStockCommand)` — tworzy nową pozycję; automatycznie wyszukuje i przypisuje `ProductDefinition` jeśli istnieje
- `Update(ModifyInStockCommand)` — aktualizuje ilość i/lub lokalizację
- `Delete(string)` — usuwa pozycję

#### Komendy (CQRS-like)

Rekordy C# przekazujące dane z kontrolera do serwisu:

| Komenda | Pola |
|---|---|
| `AddTypeCatalogCommand` | `Name`, `Unit`, `Category` |
| `ModifyTypeCatalogCommand` | `Name`, `Unit?`, `Category?` |
| `AddToStockCommand` | `Name`, `Amount`, `Location` |
| `ModifyInStockCommand` | `Name`, `Amount?`, `Location?` |

#### Modele żądań (Request Models)

DTO przyjmowane z ciała żądania HTTP:

| Model | Pola |
|---|---|
| `CreateProductDefinitionRequest` | `Name`, `Unit`, `Category` |
| `UpdateProductDefinitionRequest` | `Unit?`, `Category?` |
| `CreateStockItemRequest` | `Name`, `Amount`, `Location` |
| `UpdateStockItemRequest` | `Amount?`, `Location?` |

---

### Kitchen.Infrastructure — Infrastruktura

Implementuje dostęp do danych. Zależy od `Core` (implementuje jego interfejsy) i **nie jest znana** warstwie `Application` ani `Core`.

#### KitchenDbContext

`DbContext` dla Entity Framework Core. Zawiera dwa `DbSet`:
- `StockItems`
- `ProductDefinitions`

Konfiguracje encji ładowane są automatycznie z assembly (`ApplyConfigurationsFromAssembly`).

#### Konfiguracje EF Core

**`ProductDefinitionConfiguration`:**
- Klucz główny: `Name` (konwersja `ProductName ↔ string`)
- `Unit` i `Category` przechowywane jako `int`

**`StockItemConfiguration`:**
- Klucz główny: `Id` (konwersja `StockItemId ↔ Guid`)
- `Name` z unikalnym indeksem
- `Location` przechowywana jako `int`
- Relacja FK: `TypeName → ProductDefinition.Name` (opcjonalna)

#### Repozytoria

| Klasa | Interfejs |
|---|---|
| `PostgresStockItemRepository` | `IStockItemRepository` |
| `PostgresProductDefinitionRepository` | `IProductDefinitionRepository` |

Oba używają `AsNoTracking()` przy odczycie dla wydajności. `PostgresStockItemRepository` dodatkowo oferuje metody `GetAllWithDetails()` i `GetByNameWithDetails()` z `Include(i => i.Type)`.

#### DatabaseInitBackgroundService

`IHostedService` uruchamiany przy starcie aplikacji:
1. Stosuje wszystkie oczekujące migracje EF Core (`Database.Migrate()`)
2. Jeśli tabela `StockItems` jest pusta — dodaje przykładowe dane testowe

---

### Kitchen.Api — Warstwa prezentacji

#### Kontrolery

**`StockItemsController`** (`/api/stockitems`):

| Metoda | Route | Działanie |
|---|---|---|
| GET | `/` | `_inventoryService.GetAll()` |
| GET | `/{name}` | `_inventoryService.GetByName(name)` lub 404 |
| POST | `/` | Tworzy `AddToStockCommand`, wywołuje `Add()`, zwraca 201 |
| PUT | `/{name}` | Tworzy `ModifyInStockCommand`, wywołuje `Update()`, zwraca 204 |
| DELETE | `/{name}` | Wywołuje `Delete()`, zwraca 204 |

**`ProductDefinitionsController`** (`/api/productdefinitions`):

| Metoda | Route | Działanie |
|---|---|---|
| GET | `/` | `_catalogService.GetAll()` |
| POST | `/` | Tworzy `AddTypeCatalogCommand`, wywołuje `Add()`, zwraca 201 |
| PUT | `/{name}` | Tworzy `ModifyTypeCatalogCommand`, wywołuje `Update()`, zwraca 204 |
| DELETE | `/{name}` | Wywołuje `Delete()`, zwraca 204 |

#### ExceptionHandlingMiddleware

Globalny handler wyjątków przechwytujący wszystkie nieobsłużone wyjątki i mapujący je na kody HTTP. Szczegóły: [README — Obsługa błędów](../README.md#️-obsługa-błędów).

#### UnitTypeConverter

Niestandardowy `JsonConverter<UnitType>` obsługujący wielojęzyczne aliasy podczas deserializacji:

| Wejście (JSON) | Wynik |
|---|---|
| `"szt"`, `"sztuk"`, `"pieces"` | `UnitType.Pieces` |
| `"kg"`, `"kilograms"` | `UnitType.Kilograms` |
| `"l"`, `"liters"`, `"litry"` | `UnitType.Liters` |

Podczas serializacji zwraca skrót z atrybutu `[Description]` (np. `"kg"`).

---

## Rejestracja zależności

Każda warstwa dostarcza metodę rozszerzającą `IServiceCollection`:

| Metoda | Warstwa | Rejestruje |
|---|---|---|
| `AddCore()` | Core | *(brak — zarezerwowane na przyszłość)* |
| `AddApplication()` | Application | `ICatalogService`, `IInventoryService` (Scoped) |
| `AddInfrastructure()` | Infrastructure | DbContext, repozytoria (Scoped), `DatabaseInitBackgroundService` |
