# 📡 Dokumentacja API

Bazowy URL: `http://localhost:5099/api`

Swagger UI dostępny pod: `http://localhost:5099/swagger` (tylko w trybie `Development`)

Wszystkie żądania i odpowiedzi używają formatu **JSON** (`Content-Type: application/json`).

> ⚠️ **Uwaga o kształcie JSON-a:** `StockItem.Name`, `ProductDefinition.Name` i `StockItem.DefinitionName` są typu `ProductName` (value object bez własnego `JsonConverter`), więc w odpowiedziach serializują się jako obiekt `{ "value": "..." }`, nie jako zwykły string. To samo dotyczy `StockItem.Id` (`StockItemId` → `{ "value": "<guid>" }`). W żądaniach `POST`/`PUT` pole `name` to zwykły `string` — asymetria między tym, co się wysyła, a tym, co się dostaje z powrotem. Przykłady poniżej odzwierciedlają rzeczywisty kształt odpowiedzi API.

---

## StockItems — Zapasy

### GET `/api/stockitems`

Pobiera listę wszystkich pozycji z zapasów (z powiązaną `ProductDefinition`, jeśli istnieje).

**Odpowiedź `200 OK`:**

```json
[
  {
    "id": { "value": "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
    "name": { "value": "Mleko" },
    "amount": 2.5,
    "location": "Fridge",
    "definitionName": { "value": "Mleko" },
    "definition": {
      "name": { "value": "Mleko" },
      "unit": "l",
      "category": "Dairy"
    },
    "expirationDate": "2026-07-15"
  }
]
```

> **Uwaga:** `definition.unit` w zagnieżdżonej `ProductDefinition` wraca jako skrót (`"l"`, `"kg"`, `"szt"`, `"-"`) — przyczyna opisana w sekcji [UnitType](#unittype). Jeśli pozycja nie ma powiązanej definicji: `"definitionName": null, "definition": null`. Jeśli nie ma ustawionej daty ważności: `"expirationDate": null`.

---

### GET `/api/stockitems/{id:guid}`

Pobiera pojedynczą pozycję po `Id`.

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `200 OK` | Znaleziono — zwraca obiekt `StockItem` (kształt jak wyżej) |
| `404 Not Found` | Brak pozycji o podanym `Id` |

---

### GET `/api/stockitems/{name}`

Pobiera **wszystkie** pozycje o podanej nazwie — `StockItem.Name` nie jest unikalny (to samo mleko może leżeć w lodówce i w spiżarni jako dwie osobne pozycje).

**Parametry ścieżki:**

| Parametr | Typ | Opis |
|---|---|---|
| `name` | `string` | Nazwa pozycji |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `200 OK` | Zwraca tablicę pasujących `StockItem` |
| `404 Not Found` | Brak pozycji o podanej nazwie |

---

### POST `/api/stockitems`

Dodaje nową pozycję do zapasów.

**Ciało żądania:**

```json
{
  "name": "Mleko",
  "amount": 2.5,
  "location": "Fridge",
  "expirationDate": "2026-07-15"
}
```

| Pole | Typ | Wymagane | Opis |
|---|---|---|---|
| `name` | `string` | ✅ | Nazwa pozycji (nie może być pusta ani zaczynać się od cyfry) |
| `amount` | `double` | ✅ | Ilość (≥ 0) |
| `location` | `StorageLocation` | ❌ | Miejsce przechowywania (domyślnie: `Unspecified`) |
| `expirationDate` | `date` (`yyyy-MM-dd`) | ❌ | Data ważności; brak walidacji "nie w przeszłości" — przeterminowana pozycja to nadal realna pozycja |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `201 Created` | Pozycja została dodana |
| `400 Bad Request` | Nieprawidłowe dane wejściowe |

> **Uwaga:** Ciało odpowiedzi `201` to **echo wysłanego żądania** (`CreateStockItemRequest`), nie zapisana encja — więc wygenerowane `Id` nie wraca w odpowiedzi. Aby je poznać, należy pobrać pozycję przez `GET /api/stockitems/{name}`.
>
> Jeśli istnieje `ProductDefinition` o tej samej nazwie, zostanie automatycznie powiązana (`InventoryService.Add`).

---

### PUT `/api/stockitems/{id:guid}`

Aktualizuje istniejącą pozycję po `Id`. Wszystkie pola są opcjonalne — podaj tylko te, które chcesz zmienić.

**Ciało żądania:**

```json
{
  "name": "Mleko 2%",
  "amount": 5.0,
  "location": "Pantry",
  "expirationDate": "2026-07-20"
}
```

| Pole | Typ | Wymagane | Opis |
|---|---|---|---|
| `name` | `string?` | ❌ | Nowa nazwa |
| `amount` | `double?` | ❌ | Nowa ilość (≥ 0) |
| `location` | `StorageLocation?` | ❌ | Nowe miejsce przechowywania |
| `expirationDate` | `date?` | ❌ | Nowa data ważności |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `204 No Content` | Zaktualizowano pomyślnie |
| `400 Bad Request` | Nieprawidłowe dane |
| `404 Not Found` | Brak pozycji o podanym `Id` |

---

### DELETE `/api/stockitems/{id:guid}`

Usuwa pozycję z zapasów.

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `204 No Content` | Usunięto pomyślnie |
| `404 Not Found` | Brak pozycji o podanym `Id` |

---

## ProductDefinitions — Katalog typów produktów

### GET `/api/productdefinitions`

Pobiera listę wszystkich definicji produktów.

**Odpowiedź `200 OK`:**

```json
[
  {
    "name": { "value": "Mleko" },
    "unit": "l",
    "category": "Dairy"
  }
]
```

> `unit` wraca jako skrót (`"l"`), nie pełna nazwa — patrz [UnitType](#unittype). `category` nadal jako pełna nazwa (`"Dairy"`), bo `Category` nie ma własnego konwertera z aliasami.

---

### GET `/api/productdefinitions/{name}`

Pobiera pojedynczą definicję po nazwie.

**Parametry ścieżki:**

| Parametr | Typ | Opis |
|---|---|---|
| `name` | `string` | Nazwa definicji |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `200 OK` | Znaleziono — zwraca obiekt `ProductDefinition` (kształt jak wyżej) |
| `404 Not Found` | Brak definicji o podanej nazwie |

---

### POST `/api/productdefinitions`

Tworzy nową definicję produktu.

**Ciało żądania:**

```json
{
  "name": "Mleko",
  "unit": "Liters",
  "category": "Dairy"
}
```

| Pole | Typ | Wymagane | Opis |
|---|---|---|---|
| `name` | `string` | ✅ | Unikalna nazwa produktu (klucz główny) |
| `unit` | `UnitType` | ✅ | Jednostka miary |
| `category` | `Category` | ✅ | Kategoria produktu |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `201 Created` | Definicja została dodana; ciało to echo komendy (`name` jako zwykły string, nie `{ "value" }`) |
| `400 Bad Request` | Nieprawidłowe dane |
| `409 Conflict` | Definicja o tej nazwie już istnieje |

> **Uwaga:** jeśli w bazie są już pozycje `StockItem` o tej samej nazwie bez przypisanej definicji, `CatalogService.Add` automatycznie je z nią połączy (`LinkToExistingStockItems`).

---

### PUT `/api/productdefinitions/{name}`

Aktualizuje istniejącą definicję produktu.

**Ciało żądania:**

```json
{
  "unit": "Kilograms",
  "category": "DryGoods"
}
```

| Pole | Typ | Wymagane | Opis |
|---|---|---|---|
| `unit` | `UnitType?` | ❌ | Nowa jednostka miary |
| `category` | `Category?` | ❌ | Nowa kategoria |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `204 No Content` | Zaktualizowano pomyślnie |
| `400 Bad Request` | Nieprawidłowe dane |
| `404 Not Found` | Brak definicji o podanej nazwie |

---

### DELETE `/api/productdefinitions/{name}`

Usuwa definicję produktu.

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `204 No Content` | Usunięto pomyślnie |
| `404 Not Found` | Brak definicji o podanej nazwie |

---

## Dozwolone wartości enumeracji

### UnitType

Wszystkie miejsca, gdzie `UnitType` pojawia się w JSON-ie (`ProductDefinition.Unit`, `Create-`/`UpdateProductDefinitionRequest.Unit`), używają globalnie zarejestrowanego `UnitTypeConverter` (`Kitchen.Api/Serialization/UnitTypeConverter.cs`, dodany w `Program.cs`). Żadna z tych właściwości nie ma własnego, nadpisującego atrybutu `[JsonConverter(typeof(JsonStringEnumConverter))]`.

**Odczyt (co wolno wysłać, bez rozróżniania wielkości liter):**

| Wartość JSON | Enum |
|---|---|
| `"-"` | `Unspecified` |
| `"szt"`, `"sztuk"`, `"pieces"` | `Pieces` |
| `"kg"`, `"kilograms"` | `Kilograms` |
| `"l"`, `"liters"`, `"litry"` | `Liters` |

**Zapis (co wraca w odpowiedzi):** skrót z atrybutu `[Description]` — `"-"`, `"szt"`, `"kg"`, `"l"`, nie pełna nazwa enuma.

> ⚠️ **Uwaga:** `UnitTypeConverter.Read` nie rzuca wyjątku dla nierozpoznanej wartości — cicho zwraca `UnitType.Unspecified`. Wysłanie literówki (np. `"kilo"` zamiast `"kilograms"`) nie skończy się błędem `400`, tylko zapisze produkt z jednostką `Unspecified`.

### Category

| Wartość JSON | Enum |
|---|---|
| `"Unspecified"` | `Unspecified` |
| `"Meat"` | `Meat` |
| `"Vegetables"` | `Vegetables` |
| `"Dairy"` | `Dairy` |
| `"DryGoods"` | `DryGoods` |
| `"Spices"` | `Spices` |
| `"Other"` | `Other` |

Tak samo jak `UnitType`: standardowy `JsonStringEnumConverter`, bez aliasów.

### StorageLocation

| Wartość JSON | Enum |
|---|---|
| `"Unspecified"` | `Unspecified` (0) |
| `"Fridge"` | `Fridge` (1) |
| `"Freezer"` | `Freezer` (2) |
| `"Pantry"` | `Pantry` (3) |

Zawsze string (standardowy `JsonStringEnumConverter`), zarówno w żądaniach, jak i w odpowiedziach `GET /api/stockitems*` — `StockItem.Location` ma własny `[JsonConverter(typeof(JsonStringEnumConverter))]`, więc kształt jest spójny w obie strony.

---

## Format błędów

Wszystkie błędy zwracają JSON w jednolitej strukturze (`Kitchen.Infrastructure.Middleware.ExceptionMiddleware`):

```json
{
  "code": "product_definition_not_found",
  "message": "Czytelny komunikat błędu"
}
```

`code` to nazwa klasy wyjątku bez przyrostka `Exception`, w `snake_case` (`Humanizer.Underscore()`).

| Wyjątek | `code` | HTTP |
|---|---|---|
| `StockItemNotFoundException` | `stock_item_not_found` | `404 Not Found` |
| `ProductDefinitionNotFoundException` | `product_definition_not_found` | `404 Not Found` |
| `ProductDefinitionAlreadyExistsException` | `product_definition_already_exists` | `409 Conflict` |
| `InvalidProductNameException` | `invalid_product_name` | `400 Bad Request` |
| `IncorrectAmountException` | `incorrect_amount` | `400 Bad Request` |
| `UnknownLocationException` | `unknown_location` | `400 Bad Request` |
| `UnknownCategoryException` | `unknown_category` | `400 Bad Request` |
| `UnknownUnitTypeException` | `unknown_unit_type` | `400 Bad Request` |
| Pozostałe `KitchenApiException` | *(nazwa wyjątku)* | `400 Bad Request` |
| Nieoczekiwane błędy | *(nazwa wyjątku lub `exception`)* | `500 Internal Server Error` — logowane przez `ILogger<ExceptionMiddleware>` |
