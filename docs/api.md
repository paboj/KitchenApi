# 📡 Dokumentacja API

Bazowy URL: `http://localhost:5099/api`

Swagger UI dostępny pod: `http://localhost:5099/swagger` (tylko w trybie `Development`)

Wszystkie żądania i odpowiedzi używają formatu **JSON** (`Content-Type: application/json`).

---

## StockItems — Zapasy

### GET `/api/stockitems`

Pobiera listę wszystkich pozycji z zapasów.

**Odpowiedź `200 OK`:**

```json
[
  {
    "id": { "value": "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
    "name": "Mleko",
    "amount": 2.5,
    "location": "Fridge",
    "typeName": "Mleko",
    "type": null
  }
]
```

---

### GET `/api/stockitems/{name}`

Pobiera pojedynczą pozycję z zapasów po nazwie.

**Parametry ścieżki:**

| Parametr | Typ | Opis |
|---|---|---|
| `name` | `string` | Nazwa pozycji |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `200 OK` | Znaleziono — zwraca obiekt `StockItem` |
| `404 Not Found` | Brak pozycji o podanej nazwie |

---

### POST `/api/stockitems`

Dodaje nową pozycję do zapasów.

**Ciało żądania:**

```json
{
  "name": "Mleko",
  "amount": 2.5,
  "location": "Fridge"
}
```

| Pole | Typ | Wymagane | Opis |
|---|---|---|---|
| `name` | `string` | ✅ | Nazwa pozycji (nie może być pusta ani zaczynać się od cyfry) |
| `amount` | `double` | ✅ | Ilość (≥ 0) |
| `location` | `StorageLocation` | ❌ | Miejsce przechowywania (domyślnie: `Unspecified`) |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `201 Created` | Pozycja została dodana |
| `400 Bad Request` | Nieprawidłowe dane wejściowe |

> **Uwaga:** Jeśli istnieje `ProductDefinition` o tej samej nazwie, zostanie automatycznie powiązana.

---

### PUT `/api/stockitems/{name}`

Aktualizuje istniejącą pozycję w zapasach. Wszystkie pola są opcjonalne — podaj tylko te, które chcesz zmienić.

**Parametry ścieżki:**

| Parametr | Typ | Opis |
|---|---|---|
| `name` | `string` | Nazwa pozycji do zaktualizowania |

**Ciało żądania:**

```json
{
  "amount": 5.0,
  "location": "Pantry"
}
```

| Pole | Typ | Wymagane | Opis |
|---|---|---|---|
| `amount` | `double?` | ❌ | Nowa ilość (≥ 0) |
| `location` | `StorageLocation?` | ❌ | Nowe miejsce przechowywania |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `204 No Content` | Zaktualizowano pomyślnie |
| `400 Bad Request` | Nieprawidłowe dane |
| `404 Not Found` | Brak pozycji o podanej nazwie |

---

### DELETE `/api/stockitems/{name}`

Usuwa pozycję z zapasów.

**Parametry ścieżki:**

| Parametr | Typ | Opis |
|---|---|---|
| `name` | `string` | Nazwa pozycji do usunięcia |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `204 No Content` | Usunięto pomyślnie |
| `404 Not Found` | Brak pozycji o podanej nazwie |

---

## ProductDefinitions — Katalog typów produktów

### GET `/api/productdefinitions`

Pobiera listę wszystkich definicji produktów.

**Odpowiedź `200 OK`:**

```json
[
  {
    "name": "Mleko",
    "unit": "l",
    "category": "Dairy"
  }
]
```

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
| `name` | `string` | ✅ | Unikalna nazwa produktu |
| `unit` | `UnitType` | ✅ | Jednostka miary |
| `category` | `Category` | ✅ | Kategoria produktu |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `201 Created` | Definicja została dodana |
| `400 Bad Request` | Nieprawidłowe dane |
| `409 Conflict` | Definicja o tej nazwie już istnieje |

---

### PUT `/api/productdefinitions/{name}`

Aktualizuje istniejącą definicję produktu.

**Parametry ścieżki:**

| Parametr | Typ | Opis |
|---|---|---|
| `name` | `string` | Nazwa definicji do zaktualizowania |

**Ciało żądania:**

```json
{
  "unit": "Kilograms",
  "category": "Grains"
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

**Parametry ścieżki:**

| Parametr | Typ | Opis |
|---|---|---|
| `name` | `string` | Nazwa definicji do usunięcia |

**Odpowiedzi:**

| Kod | Opis |
|---|---|
| `204 No Content` | Usunięto pomyślnie |
| `404 Not Found` | Brak definicji o podanej nazwie |

---

## Dozwolone wartości enumeracji

### UnitType

Akceptowane wartości (wielkość liter nieistotna):

| Wartość JSON | Enum |
|---|---|
| `"Unspecified"` lub `"-"` | `Unspecified` |
| `"Pieces"`, `"szt"`, `"sztuk"` | `Pieces` |
| `"Kilograms"`, `"kg"` | `Kilograms` |
| `"Liters"`, `"liters"`, `"litry"`, `"l"` | `Liters` |

Odpowiedzi zwracają skróty: `"-"`, `"szt"`, `"kg"`, `"l"`.

### StorageLocation

| Wartość | Opis |
|---|---|
| `"Unspecified"` | Nieokreślone |
| `"Fridge"` | Lodówka |
| `"Freezer"` | Zamrażarka |
| `"Pantry"` | Spiżarnia |

---

## Format błędów

Wszystkie błędy zwracają JSON w jednolitej strukturze:

```json
{
  "error": "Czytelny komunikat błędu",
  "code": "BadRequest",
  "type": "NazwaKlasyWyjątku"
}
```
