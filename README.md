# 🍽️ Kitchen.Api

REST API do zarządzania spiżarnią kuchenną — pozwala śledzić stan zapasów (`StockItems`) oraz katalog definicji produktów (`ProductDefinitions`).

Zbudowane w **.NET 8** z wykorzystaniem architektury **Clean Architecture**, bazy danych **PostgreSQL** i **Entity Framework Core**.

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

---

## ✅ Funkcjonalności

- Zarządzanie **zapasami** (dodawanie, edycja, usuwanie, przeglądanie produktów w spiżarni)
- Zarządzanie **katalogiem definicji produktów** (typy składników z jednostką miary i kategorią)
- Automatyczna inicjalizacja bazy danych przy starcie aplikacji
- Globalna obsługa błędów z czytelnymi komunikatami JSON
- Dokumentacja Swagger dostępna w trybie developerskim

---

## 🏗️ Architektura

Projekt stosuje **Clean Architecture** z podziałem na cztery warstwy:

```
Kitchen.Api              ← Warstwa prezentacji (kontrolery, middleware)
Kitchen.Application      ← Logika aplikacji (serwisy, komendy, modele żądań)
Kitchen.Core             ← Domena (encje, value objects, wyjątki, interfejsy repozytoriów)
Kitchen.Infrastructure   ← Infrastruktura (EF Core, PostgreSQL, repozytoria, migracje)
```

Zależności płyną tylko do wewnątrz — `Infrastructure` i `Application` zależą od `Core`, `Api` zależy od `Application`.

Szczegółowy opis warstw: [docs/architektura.md](docs/architektura.md)

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

Możesz użyć Dockera:

```bash
docker run --name kitchendb \
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

Migracje bazy danych są **automatycznie stosowane** przy starcie aplikacji (`DatabaseInitBackgroundService`).

### 5. Otwórz Swagger UI

```
http://localhost:5099/swagger
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

Pełna dokumentacja endpointów: [docs/api.md](docs/api.md)

### StockItems — zapasy

| Metoda | Endpoint | Opis |
|---|---|---|
| `GET` | `/api/stockitems` | Pobierz wszystkie pozycje z zapasów |
| `GET` | `/api/stockitems/{name}` | Pobierz pozycję po nazwie |
| `POST` | `/api/stockitems` | Dodaj nową pozycję do zapasów |
| `PUT` | `/api/stockitems/{name}` | Zaktualizuj pozycję |
| `DELETE` | `/api/stockitems/{name}` | Usuń pozycję |

### ProductDefinitions — katalog typów produktów

| Metoda | Endpoint | Opis |
|---|---|---|
| `GET` | `/api/productdefinitions` | Pobierz wszystkie definicje produktów |
| `POST` | `/api/productdefinitions` | Dodaj nową definicję produktu |
| `PUT` | `/api/productdefinitions/{name}` | Zaktualizuj definicję produktu |
| `DELETE` | `/api/productdefinitions/{name}` | Usuń definicję produktu |

---

## 🗂️ Model danych

### Enumy

| Enum | Wartości |
|---|---|
| `UnitType` | `Unspecified`, `Pieces` (szt), `Kilograms` (kg), `Liters` (l) |
| `Category` | `Unspecified` + zdefiniowane kategorie |
| `StorageLocation` | `Unspecified`, `Fridge`, `Freezer`, `Pantry` |

### Value Objects

- **`ProductName`** — nazwa produktu; nie może być pusta ani zaczynać się od cyfry
- **`StockItemId`** — GUID identyfikujący pozycję zapasów

---

## ⚠️ Obsługa błędów

Wszystkie błędy zwracają spójną strukturę JSON:

```json
{
  "error": "Treść komunikatu błędu",
  "code": "BadRequest",
  "type": "NazwaWyjątku"
}
```

| Wyjątek | Kod HTTP |
|---|---|
| `StockItemNotFoundException` | `404 Not Found` |
| `InvalidProductNameException` | `400 Bad Request` |
| `IncorrectAmountException` | `400 Bad Request` |
| `UnknownLocationException` | `400 Bad Request` |
| `UnknownUnitTypeException` | `400 Bad Request` |
| `ProductDefinitionAlreadyExistsException` | `409 Conflict` |
| Pozostałe `KitchenApiException` | `400 Bad Request` |
| Nieoczekiwane błędy | `500 Internal Server Error` |

---

## 📁 Struktura projektu

```
Kitchen.Api/
├── Controllers/
│   ├── IngredientsController.cs      # Endpointy StockItems
│   └── IngredientTypesController.cs  # Endpointy ProductDefinitions
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Serialization/
│   └── UnitTypeConverter.cs
└── Program.cs

Kitchen.Application/
├── Commands/
│   ├── CatalogCommands.cs
│   └── InventoryCommands.cs
├── Models/Requests/
│   ├── CreateProductDefinitionRequest.cs
│   ├── CreateStockItemRequest.cs
│   ├── UpdateProductDefinitionRequest.cs
│   └── UpdateStockItemRequest.cs
└── Services/
    ├── CatalogService.cs
    └── InventoryService.cs

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
│       ├── KitchenApiException.cs
│       └── (pochodne wyjątki)
├── Repositories/
│   ├── IIngredientRepository.cs
│   └── IProductDefinitionRepository.cs
└── ValueObjects/
    ├── ProductName.cs
    └── StockItemId.cs

Kitchen.Infrastructure/
├── BackgroundServices/
│   └── DatabaseInitBackgroundService.cs
└── DAL/
    ├── Configurations/
    │   ├── ProductDefinitionConfiguration.cs
    │   └── StockItemConfiguration.cs
    ├── Migrations/
    ├── Repositories/
    │   ├── PostgresProductDefinitionRepository.cs
    │   └── PostgresStockItemRepository.cs
    └── KitchenDbContext.cs
```

---

## 🧪 Testy

Projekt zawiera warstwę `Kitchen.Tests.Unit`. Aby uruchomić testy:

```bash
dotnet test
```
