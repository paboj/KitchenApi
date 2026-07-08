# 🍽️ Kitchen.Api

REST API do zarządzania spiżarnią kuchenną — pozwala śledzić stan zapasów (`StockItems`) oraz katalog definicji produktów (`ProductDefinitions`).

Zbudowane w **.NET 8** z wykorzystaniem architektury **Clean Architecture**, bazy danych **PostgreSQL** (Npgsql.EntityFrameworkCore.PostgreSQL) i **Entity Framework Core**.

---

## 📋 Spis treści

- [Funkcjonalności](#-funkcjonalności)
- [Architektura](#-architektura)
- [Wymagania](#-wymagania)
- [Uruchomienie](#-uruchomienie)
- [Konfiguracja](#-konfiguracja)
- [Endpointy API](#-endpointy-api)
- [Model danych](#-model-danych)
- [Obsługa błędów](#-obsługa-błędów)
- [Struktura projektu](#-struktura-projektu)
- [Testy](#-testy)

---

## ✅ Funkcjonalności

- Zarządzanie **zapasami** (`StockItems`): dodawanie, edycja i usuwanie po `Id` (GUID), przeglądanie wszystkich pozycji oraz wyszukiwanie po nazwie
- Zarządzanie **katalogiem definicji produktów** (`ProductDefinitions`): typy składników z jednostką miary i kategorią
- Automatyczne powiązanie `StockItem ↔ ProductDefinition` po nazwie — w obie strony (przy dodaniu zapasu, jeśli istnieje pasująca definicja, oraz przy dodaniu definicji, jeśli istnieją niepowiązane zapasy o tej samej nazwie)
- Automatyczna inicjalizacja bazy danych (migracje + dane testowe) przy starcie aplikacji
- Globalna obsługa błędów z czytelnymi komunikatami JSON
- CORS skonfigurowany dla lokalnego frontendu deweloperskiego (`http://localhost:5173`)
- Dokumentacja Swagger dostępna w trybie developerskim

---

## 🏗️ Architektura

Projekt stosuje **Clean Architecture** z podziałem na cztery warstwy:

```
Kitchen.Api              ← Warstwa prezentacji (kontrolery, DI)
Kitchen.Application      ← Logika aplikacji (serwisy, komendy, modele żądań)
Kitchen.Core             ← Domena (encje, value objects, wyjątki, interfejsy repozytoriów)
Kitchen.Infrastructure   ← Infrastruktura (EF Core, PostgreSQL, repozytoria, migracje, middleware wyjątków)
```

Zależności płyną tylko do wewnątrz — `Infrastructure` i `Application` zależą od `Core`, `Api` zależy od `Application`.

Pogłębiony opis warstw znajduje się w [docs/architektura.md](docs/architektura.md), natomiast pełny opis endpointów (razem z dokładnym kształtem JSON-a) w [docs/api.md](docs/api.md).

---

## 📦 Wymagania

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/) (wersja 13+)
- (opcjonalnie) [Docker](https://www.docker.com/) do uruchomienia bazy danych

---

## 🚀 Uruchomienie

### 1. Sklonuj repozytorium

```bash
git clone <adres-repozytorium>
cd Kitchen.Api
```

### 2. Uruchom bazę danych PostgreSQL

W repozytorium znajduje się gotowy `docker-compose.yml`:

```bash
docker compose up -d
```

Alternatywnie, ręcznie:

```bash
docker run --name kitchen-api-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=KitchenDb \
  -p 5432:5432 \
  -d postgres
```

### 3. Skonfiguruj connection string

W pliku `appsettings.json` (lub przez zmienne środowiskowe):

```json
{
  "database": {
    "ConnectionString": "Host=localhost;Database=KitchenDb;Username=postgres;Password=postgres"
  }
}
```

### 4. Uruchom aplikację

```bash
dotnet run --project Kitchen.Api
```

Migracje bazy danych są **automatycznie stosowane** przy starcie aplikacji (`DatabaseInitBackgroundService`), a jeśli tabela `StockItems` jest pusta — dodawane są przykładowe dane testowe.

### 5. Otwórz Swagger UI

```
http://localhost:5099/swagger
```

### Praca z migracjami EF Core

Design-time tworzenie `DbContext` obsługuje `KitchenDbContextFactory` (`IDesignTimeDbContextFactory<KitchenDbContext>`), dzięki czemu CLI EF Core działa bez uruchamiania aplikacji:

```bash
dotnet ef migrations add <NazwaMigracji> --project Kitchen.Infrastructure --startup-project Kitchen.Api
```

---

## ⚙️ Konfiguracja

| Klucz | Opis | Domyślna wartość |
|---|---|---|
| `database:ConnectionString` | Connection string do PostgreSQL | `Host=localhost;Database=KitchenDb;Username=postgres;Password=postgres` |
| `Logging:LogLevel:Default` | Poziom logowania | `Information` |

W środowisku `Development` włączone są szczegółowe błędy (`DetailedErrors: true`).

---

## 📡 Endpointy API

Bazowy URL: `http://localhost:5099/api`

### StockItems — zapasy

| Metoda | Endpoint | Opis |
|---|---|---|
| `GET` | `/api/stockitems` | Pobierz wszystkie pozycje z zapasów (z powiązaną `ProductDefinition`, jeśli istnieje) |
| `GET` | `/api/stockitems/{id:guid}` | Pobierz pozycję po `Id` — `404`, jeśli nie istnieje |
| `GET` | `/api/stockitems/{name}` | Pobierz **wszystkie** pozycje o danej nazwie — `404`, jeśli brak wyników |
| `POST` | `/api/stockitems` | Dodaj nową pozycję do zapasów — `201 Created` |
| `PUT` | `/api/stockitems/{id:guid}` | Zaktualizuj pozycję po `Id` — `204 No Content` |
| `DELETE` | `/api/stockitems/{id:guid}` | Usuń pozycję po `Id` — `204 No Content` |

> **Uwaga:** `StockItem` nie ma unikalnego klucza na nazwie — ta sama nazwa może występować wielokrotnie (np. mleko w lodówce i mleko w spiżarni jako dwie osobne pozycje), dlatego identyfikacja po `Id` jest jedynym sposobem jednoznacznej aktualizacji/usunięcia pozycji.

### ProductDefinitions — katalog typów produktów

| Metoda | Endpoint | Opis |
|---|---|---|
| `GET` | `/api/productdefinitions` | Pobierz wszystkie definicje produktów |
| `GET` | `/api/productdefinitions/{name}` | Pobierz definicję po nazwie — `404`, jeśli nie istnieje |
| `POST` | `/api/productdefinitions` | Dodaj nową definicję produktu — `201 Created` |
| `PUT` | `/api/productdefinitions/{name}` | Zaktualizuj definicję produktu (klucz: nazwa) — `204 No Content` |
| `DELETE` | `/api/productdefinitions/{name}` | Usuń definicję produktu — `204 No Content` |

---

## 🗂️ Model danych

### Enumy

| Enum | Wartości |
|---|---|
| `UnitType` | `Unspecified`, `Pieces` (szt), `Kilograms` (kg), `Liters` (l) |
| `Category` | `Unspecified`, `Meat` (mięso), `Vegetables` (warzywa), `Dairy` (nabiał), `DryGoods` (sypkie), `Spices` (przyprawy), `Other` (inne) |
| `StorageLocation` | `Unspecified`, `Fridge` (lodówka), `Freezer` (zamrażarka), `Pantry` (szafki) |

`Category` jest obecnie zwykłym enumem.

### Value Objects

- **`ProductName`** — nazwa produktu; nie może być pusta ani zaczynać się od cyfry
- **`StockItemId`** — GUID identyfikujący pozycję zapasów

---

## ⚠️ Obsługa błędów

Globalny `ExceptionMiddleware` (`Kitchen.Infrastructure/Middleware`, `IMiddleware`) łapie wszystkie wyjątki, loguje je i zwraca spójną strukturę JSON:

```json
{
  "code": "product_definition_not_found",
  "message": "Treść komunikatu błędu"
}
```

`code` to nazwa klasy wyjątku bez przyrostka `Exception`, w `snake_case`.

| Wyjątek | Kod HTTP |
|---|---|
| `StockItemNotFoundException` | `404 Not Found` |
| `ProductDefinitionNotFoundException` | `404 Not Found` |
| `ProductDefinitionAlreadyExistsException` | `409 Conflict` |
| `InvalidProductNameException` | `400 Bad Request` |
| `IncorrectAmountException` | `400 Bad Request` |
| `UnknownLocationException` | `400 Bad Request` |
| `UnknownCategoryException` | `400 Bad Request` |
| `UnknownUnitTypeException` | `400 Bad Request` |
| Pozostałe `KitchenApiException` | `400 Bad Request` |
| Nieoczekiwane błędy | `500 Internal Server Error` (logowane) |

Pełna specyfikacja, w tym dokładny kształt JSON-a dla każdego endpointu: [docs/api.md](docs/api.md#format-błędów).

---

## 📁 Struktura projektu

```
Kitchen.Api/
├── Controllers/
│   ├── StockItemsController.cs         # Endpointy StockItems (GET/POST/PUT/DELETE po Id, GET po name)
│   └── ProductDefinitionsController.cs # Endpointy ProductDefinitions
├── Serialization/
│   └── UnitTypeConverter.cs            # JsonConverter<UnitType> z aliasami PL, zarejestrowany globalnie
└── Program.cs

Kitchen.Application/
├── Commands/
│   ├── CatalogCommands.cs      # AddProductDefinitionCommand, ModifyProductDefinitionCommand
│   └── InventoryCommands.cs    # AddStockItemCommand, ModifyStockItemCommand
├── Models/Requests/
│   ├── CreateProductDefinitionRequest.cs
│   ├── CreateStockItemRequest.cs
│   ├── UpdateProductDefinitionRequest.cs
│   └── UpdateStockItemRequest.cs
└── Services/
    ├── CatalogService.cs    # + LinkToExistingStockItems(), łączy nowe definicje z istniejącymi zapasami
    └── InventoryService.cs  # korzysta z wariantów *WithDetails repozytorium

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
│       ├── KitchenApiException.cs   # bazowy wyjątek domenowy
│       └── (pochodne wyjątki, w tym ProductDefinitionNotFoundException, UnknownCategoryException)
├── Repositories/
│   ├── IStockItemRepository.cs         # GetAll/GetById/GetByName + warianty WithDetails
│   └── IProductDefinitionRepository.cs
└── ValueObjects/
    ├── ProductName.cs
    └── StockItemId.cs

Kitchen.Infrastructure/
├── BackgroundServices/
│   └── DatabaseInitBackgroundService.cs  # migracje + seed danych testowych przy starcie
├── Middleware/
│   └── ExceptionMiddleware.cs   # IMiddleware, globalna obsługa błędów (patrz wyżej)
└── DAL/
    ├── Configurations/
    │   ├── ProductDefinitionConfiguration.cs
    │   └── StockItemConfiguration.cs
    ├── Migrations/
    ├── Repositories/
    │   ├── PostgresStockItemRepository.cs
    │   └── PostgresProductDefinitionRepository.cs
    ├── KitchenDbContext.cs
    ├── KitchenDbContextFactory.cs   # IDesignTimeDbContextFactory — wsparcie dla `dotnet ef`, zahardkodowany connection string
    ├── ConfigurationExtensions.cs   # GetOptions<T>() — generyczny binder appsettings
    └── PostgresOptions.cs
```

---

## 🧪 Testy

Projekt ma dwie warstwy testów: `Kitchen.Tests.Unit` (xUnit, FluentAssertions, Moq) oraz `Kitchen.Tests.Integration` (xUnit, FluentAssertions, **Testcontainers.PostgreSql** — odpala prawdziwego Postgresa w kontenerze i realne migracje).

```bash
dotnet test Kitchen.Tests.Unit
dotnet test Kitchen.Tests.Integration   # wymaga Dockera
```

CI (`.github/workflows/ci.yml`) odpala obie warstwy na PR-ach do `master` i push'ach do `dev`.

Aktualne pokrycie testów jednostkowych:

| Warstwa / obszar | Klasa testowa | Zakres |
|---|---|---|
| Domena | `StockItemTests` | konstruktor, `AdjustAmount`, `PlaceOrMove` |
| Domena | `ProductDefinitionTests` | konstruktor, `ChangeUnitType` |
| Aplikacja | `InventoryServiceTests` | `GetAll`, `GetByName`, `Add`, `Update`, `Delete` |
| Aplikacja | `CatalogServiceTests` | `Add` (w tym `LinkToExistingStockItems` i regresja na równość `ProductName`) |
| Infrastruktura | `ExceptionMiddlewareTests` | mapowanie wyjątków na kody HTTP i treść JSON |
| API | `StockItemsControllerTests` | `Create` |
| API | `ProductDefinitionsControllerTests` | `GetAll`, `Create`, `Update`, `Delete` |

Testy integracyjne (`Kitchen.Tests.Integration/Repositories/StockItemRepositoryTests`): auto-linking `StockItem` ↔ `ProductDefinition` na realnej bazie, regresja na ponowne wstawianie już istniejącej `ProductDefinition`.

Braki w pokryciu: brak testów kontrolera dla `Update`/`Delete`/`Get` w `StockItemsController`, brak testów jednostkowych na `SetName`/`AssignDefinition`/`SetExpirationDate` w `StockItemTests`.
