# 🏗️ Architektura Kitchen.Api

## Clean Architecture

Projekt oparty jest na wzorcu **Clean Architecture**, który zapewnia separację odpowiedzialności i niezależność warstw domenowych od szczegółów technicznych (bazy danych, frameworku webowego itp.).

```
┌─────────────────────────────────┐
│         Kitchen.Api             │  ← Warstwa prezentacji
│  (kontrolery, DI, Program.cs)   │
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
│  (EF Core, PostgreSQL, repos,   │
│   middleware wyjątków)          │
└─────────────────────────────────┘
```

> **Uwaga:** globalny `ExceptionMiddleware` (mapowanie wyjątków domenowych na kody HTTP) znajduje się w `Kitchen.Infrastructure`, nie w `Kitchen.Api`. To celowa decyzja architektoniczna — obsługa wyjątków jest cross-cutting concern (dotyczy każdego żądania, niezależnie od warstwy), dlatego umieszczono ją razem z pozostałą infrastrukturą przekrojową, a nie w warstwie prezentacji. `Kitchen.Api` rejestruje go przez `app.UseInfrastructure()`.

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
- `SetName(string)` — ustawia nazwę (deleguje walidację do `ProductName`)
- `ChangeUnitType(UnitType?)` — zmienia jednostkę; rzuca `UnknownUnitTypeException` przy nieprawidłowej wartości
- `SetCategory(Category?)` — ustawia kategorię; rzuca `UnknownCategoryException`

---

**`StockItem`** — konkretna pozycja w zapasach kuchennych.

| Właściwość | Typ | Opis |
|---|---|---|
| `Id` | `StockItemId` | Klucz główny (GUID) |
| `Name` | `ProductName` | Nazwa pozycji — **nieunikalna** (patrz niżej) |
| `Amount` | `double` | Ilość (musi być ≥ 0) |
| `Location` | `StorageLocation` | Miejsce przechowywania |
| `DefinitionName` | `ProductName?` | Nazwa powiązanej definicji (shadow FK w EF) |
| `Definition` | `ProductDefinition?` | Opcjonalne powiązanie z definicją produktu |
| `ExpirationDate` | `DateOnly?` | Data ważności — bez walidacji "nie w przeszłości" |

Metody domenowe:
- `SetName(string?)` — zmienia nazwę
- `AdjustAmount(double?)` — ustawia ilość; rzuca `IncorrectAmountException` gdy < 0
- `PlaceOrMove(StorageLocation?)` — ustawia lokalizację; rzuca `UnknownLocationException`
- `AssignDefinition(ProductDefinition?)` — przypisuje definicję produktu
- `SetExpirationDate(DateOnly?)` — ustawia datę ważności

`Name` nie ma unikalnego klucza — ta sama nazwa może wystąpić wielokrotnie (np. mleko w lodówce i mleko w spiżarni jako dwie osobne pozycje). Jednoznaczna identyfikacja jest możliwa wyłącznie po `Id`.

#### Value Objects

**`ProductName`** — opakowuje string z walidacją:
- nie może być pusty ani składać się z samych białych znaków
- nie może zaczynać się od cyfry
- automatycznie usuwa białe znaki z początku i końca (`.Trim()`)
- posiada niejawne konwersje `string ↔ ProductName`
- **równość po wartości** (`IEquatable<ProductName>`, porównanie `Value` przez `StringComparison.Ordinal`) — istotne dla `CatalogService.LinkToExistingStockItems`, które porównuje `ProductName` po nazwie, nie po referencji

**`StockItemId`** — `record StockItemId(Guid Value)`, klucz główny `StockItem`.

#### Enumy

| Enum | Wartości |
|---|---|
| `UnitType` | `Unspecified(0)`, `Pieces(1)`, `Kilograms(2)`, `Liters(3)` |
| `Category` | `Unspecified(0)`, `Meat(1)`, `Vegetables(2)`, `Dairy(3)`, `DryGoods(4)`, `Spices(5)`, `Other(6)` |
| `StorageLocation` | `Unspecified(0)`, `Fridge(1)`, `Freezer(2)`, `Pantry(3)` |

Każda wartość posiada atrybut `[Description]` (np. `"kg"`, `"szt"`, `"mięso"`, `"lodówka"`), zwracany przez `EnumExtensions.ToDescription()`. To osobny mechanizm od serializacji JSON — dokładny opis tego, co faktycznie trafia do odpowiedzi JSON, w [docs/api.md](./api.md#dozwolone-wartości-enumeracji).

#### Wyjątki domenowe

Wszystkie dziedziczą po `KitchenApiException : Exception`.

| Wyjątek | Znaczenie |
|---|---|
| `StockItemNotFoundException` | Szukana pozycja nie istnieje |
| `ProductDefinitionNotFoundException` | Szukana definicja nie istnieje |
| `ProductDefinitionAlreadyExistsException` | Definicja o tej nazwie już istnieje |
| `InvalidProductNameException` | Nieprawidłowa nazwa produktu |
| `IncorrectAmountException` | Ujemna ilość |
| `UnknownLocationException` | Nieznana lokalizacja |
| `UnknownUnitTypeException` | Nieznana jednostka miary |
| `UnknownCategoryException` | Nieznana kategoria |

#### Interfejsy repozytoriów

**`IStockItemRepository`:** `GetAll`, `GetById`, `GetByName`, `Add`, `Update`, `Delete` oraz warianty `GetAllWithDetails`/`GetByIdWithDetails`/`GetByNameWithDetails` (dociągają `Definition`).

**`IProductDefinitionRepository`:** `GetAll`, `GetByName`, `Add`, `Update`, `Delete`.

Zdefiniowane w Core, implementowane w Infrastructure. Dzięki temu domena nie zna szczegółów bazy danych.

---

### Kitchen.Application — Logika aplikacji

Orkiestruje przepływ danych między API a domeną. Nie zawiera logiki domenowej ani szczegółów infrastrukturalnych.

#### Serwisy

**`ICatalogService` / `CatalogService`** — zarządzanie katalogiem definicji produktów:
- `GetAll()` — zwraca wszystkie definicje
- `GetByName(string)` — szuka definicji po nazwie
- `Add(AddProductDefinitionCommand)` — sprawdza unikalność nazwy (`ProductDefinitionAlreadyExistsException`), tworzy definicję, po zapisie wywołuje `LinkToExistingStockItems`
- `Update(ModifyProductDefinitionCommand)` — szuka definicji (`ProductDefinitionNotFoundException` jeśli brak), aktualizuje jednostkę/kategorię
- `Delete(string)` — szuka definicji, usuwa
- `LinkToExistingStockItems(ProductDefinition)` — przechodzi po wszystkich `StockItem`ach i podpina tę definicję do tych, które mają tę samą nazwę i jeszcze nie mają żadnej definicji.

**`IInventoryService` / `InventoryService`** — zarządzanie zapasami:
- `GetAll()` / `GetById(Guid)` / `GetByName(string)` — korzystają z wariantów `*WithDetails` repozytorium (dociągają `Definition`)
- `Add(AddStockItemCommand)` — szuka `ProductDefinition` po nazwie i automatycznie przypisuje, jeśli istnieje
- `Update(ModifyStockItemCommand)` — szuka pozycji po `Id` (`StockItemNotFoundException` jeśli brak), aktualizuje nazwę/ilość/lokalizację/datę ważności
- `Delete(Guid)` — szuka pozycji, usuwa

#### Komendy (CQRS-like)

Rekordy C# przekazujące dane z kontrolera do serwisu:

| Komenda | Pola |
|---|---|
| `AddProductDefinitionCommand` | `Name`, `Unit`, `Category` |
| `ModifyProductDefinitionCommand` | `Name`, `Unit?`, `Category?` |
| `AddStockItemCommand` | `Name`, `Amount`, `Location`, `ExpirationDate = null` |
| `ModifyStockItemCommand` | `Id`, `Name?`, `Amount?`, `Location?`, `ExpirationDate = null` |

#### Modele żądań (Request Models)

DTO przyjmowane z ciała żądania HTTP — `Name`/`Location` tu są zwykłymi typami (`string`, string-enum), w przeciwieństwie do encji zwracanych w odpowiedziach (dokładny opis asymetrii JSON w [docs/api.md](./api.md)):

| Model | Pola |
|---|---|
| `CreateProductDefinitionRequest` | `Name`, `Unit`, `Category` |
| `UpdateProductDefinitionRequest` | `Unit?`, `Category?` |
| `CreateStockItemRequest` | `Name`, `Amount`, `Location`, `ExpirationDate?` |
| `UpdateStockItemRequest` | `Name?`, `Amount?`, `Location?`, `ExpirationDate?` |

---

### Kitchen.Infrastructure — Infrastruktura

Implementuje dostęp do danych i globalną obsługę wyjątków. Zależy od `Core` (implementuje jego interfejsy).

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
- `Name` wymagane, konwersja `ProductName ↔ string` — **bez unikalnego indeksu** (świadomie, `Name` nie jest unikalne)
- `ExpirationDate` opcjonalne
- `Location` przechowywana jako `int`
- Shadow property `DefinitionName` (`ProductName ↔ string`, opcjonalna) + relacja FK: `DefinitionName → ProductDefinition.Name` (opcjonalna, `WithMany()` — jedna definicja może mieć wiele powiązanych pozycji)

#### Repozytoria

| Klasa | Interfejs |
|---|---|
| `PostgresStockItemRepository` | `IStockItemRepository` |
| `PostgresProductDefinitionRepository` | `IProductDefinitionRepository` |

Oba używają `AsNoTracking()` przy odczycie. `PostgresStockItemRepository` dodatkowo oferuje warianty `*WithDetails` z `Include(i => i.Definition)`.

`Add`/`Update` w `PostgresStockItemRepository` sprawdzają, czy `stockItem.Definition` jest w stanie `Detached`, i w takim przypadku wywołują `Attach()` — `Definition` pochodzi zwykle z odczytu `AsNoTracking()`, więc `DbContext` go nie śledzi.

#### KitchenDbContextFactory

`IDesignTimeDbContextFactory<KitchenDbContext>` — pozwala CLI EF Core (`dotnet ef migrations add ...`) tworzyć `DbContext` bez uruchamiania aplikacji. Connection string jest zahardkodowany w tej klasie, niezależnie od `appsettings.json` — używany wyłącznie w design-time, nie w runtime.

#### DatabaseInitBackgroundService

`IHostedService` uruchamiany przy starcie aplikacji:
1. Stosuje wszystkie oczekujące migracje EF Core (`Database.MigrateAsync()`)
2. Jeśli tabela `StockItems` jest pusta — dodaje przykładowe dane testowe (6 `ProductDefinition` + 6 `StockItem`, polskie nazwy: Mleko, Jajka, Kurczak, Marchew, Ryż, Papryka mielona)

#### ExceptionMiddleware

`internal sealed class ExceptionMiddleware : IMiddleware` (namespace `Kitchen.Infrastructure.Middleware`) — globalny handler wyjątków. Loguje każdy przechwycony wyjątek (`ILogger<ExceptionMiddleware>`), mapuje typ wyjątku na kod HTTP i zwraca `{ "code", "message" }` (kod w `snake_case`, przez `Humanizer.Underscore()`). Pełna tabela mapowania: [docs/api.md — Format błędów](./api.md#format-błędów).

Rejestracja: `AddTransient<ExceptionMiddleware>()` w `AddInfrastructure()`, użycie: `app.UseMiddleware<ExceptionMiddleware>()` w `UseInfrastructure()` — obie w `Kitchen.Infrastructure/Extensions.cs`.

#### Migracje

| Migracja | Opis |
|---|---|
| `Initial` (2026-07-05) | Punkt startowy po połączeniu wszystkich wcześniejszych migracji w jedną |
| `AddExpirationDateToStockItem` (2026-07-05) | Kolumna `ExpirationDate` na `StockItems` |
| `RenameTypeToDefinition` (2026-07-06) | Zmiana kolumny/FK `TypeName` → `DefinitionName` |

---

### Kitchen.Api — Warstwa prezentacji

#### Kontrolery

**`StockItemsController`** (`/api/stockitems`):

| Metoda | Route | Działanie |
|---|---|---|
| GET | `/` | `_inventoryService.GetAll()` |
| GET | `/{id:guid}` | `_inventoryService.GetById(id)` lub 404 |
| GET | `/{name}` | `_inventoryService.GetByName(name)` lub 404 (jeśli pusta kolekcja) |
| POST | `/` | Tworzy `AddStockItemCommand`, wywołuje `Add()`, zwraca 201 z echem żądania |
| PUT | `/{id:guid}` | Tworzy `ModifyStockItemCommand`, wywołuje `Update()`, zwraca 204 |
| DELETE | `/{id:guid}` | Wywołuje `Delete()`, zwraca 204 |

**`ProductDefinitionsController`** (`/api/productdefinitions`):

| Metoda | Route | Działanie |
|---|---|---|
| GET | `/` | `_catalogService.GetAll()` |
| GET | `/{name}` | `_catalogService.GetByName(name)` lub 404 |
| POST | `/` | Tworzy `AddProductDefinitionCommand`, wywołuje `Add()`, zwraca 201 z echem komendy (`Location` header wskazuje na `Get`) |
| PUT | `/{name}` | Tworzy `ModifyProductDefinitionCommand`, wywołuje `Update()`, zwraca 204 |
| DELETE | `/{name}` | Wywołuje `Delete()`, zwraca 204 |

#### Serialization/UnitTypeConverter.cs

Niestandardowy `JsonConverter<UnitType>` z aliasami PL (`szt`, `kg`, `l`, `litry`...), zarejestrowany globalnie w `Program.cs` (aliasy na wejściu, skrót z `[Description]` na wyjściu). Nierozpoznana wartość na wejściu cicho staje się `UnitType.Unspecified`, zamiast zwrócić błąd — opisane szczegółowo w [docs/api.md](./api.md#unittype).

#### Program.cs — kolejność rejestracji

```
AddControllers().AddJsonOptions(+ UnitTypeConverter)
AddCore() / AddApplication() / AddInfrastructure(config)
AddSwaggerGen()
AddCors("FrontendCorsPolicy" → http://localhost:5173)
─────────────
UseInfrastructure()   // rejestruje ExceptionMiddleware
Swagger (tylko Development)
UseCors("FrontendCorsPolicy")
MapControllers()
```

---

## Rejestracja zależności

Każda warstwa dostarcza metodę rozszerzającą `IServiceCollection`:

| Metoda | Warstwa | Rejestruje |
|---|---|---|
| `AddCore()` | Core | *(brak — zarezerwowane na przyszłość)* |
| `AddApplication()` | Application | `ICatalogService`, `IInventoryService` (Scoped) |
| `AddInfrastructure(config)` | Infrastructure | `KitchenDbContext` + repozytoria (Scoped), `DatabaseInitBackgroundService` (Hosted), `ExceptionMiddleware` (Transient) |
