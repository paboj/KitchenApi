# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Kitchen.Api — a REST API for managing a kitchen pantry: inventory (`StockItems`) and a catalog of product definitions (`ProductDefinitions`). .NET 8 / ASP.NET Core Web API, EF Core 8 + Npgsql, PostgreSQL 16.

## Commands

```bash
# Build
dotnet build

# Run (Swagger at http://localhost:5099/swagger in Development)
dotnet run --project Kitchen.Api

# Unit tests
dotnet test Kitchen.Tests.Unit

# Integration tests (requires Docker — spins up a real Postgres via Testcontainers)
dotnet test Kitchen.Tests.Integration

# Run a single test
dotnet test Kitchen.Tests.Unit --filter "FullyQualifiedName~InventoryServiceTests.Add"

# EF Core migrations (KitchenDbContextFactory supplies design-time DbContext, no running app needed)
dotnet ef migrations add <MigrationName> --project Kitchen.Infrastructure --startup-project Kitchen.Api

# CI also runs this check — fails if the model and the latest migration have diverged
dotnet ef migrations has-pending-model-changes --project Kitchen.Infrastructure --startup-project Kitchen.Api

# Local Postgres via Docker
docker compose up -d
```

One-time per machine, before running or migrating:
```bash
dotnet user-secrets set "database:ConnectionString" "Host=localhost;Database=KitchenDb;Username=postgres;Password=postgres" --project Kitchen.Api
```
`appsettings.json` intentionally ships with an empty connection string — never put a real one there. `KitchenDbContextFactory` reads the same user-secret directly for the EF CLI and throws a clear error if it's unset.

CI (`.github/workflows/ci.yml`) runs on PRs to `master` and pushes to `dev`: build → unit tests → integration tests → `has-pending-model-changes`.

## Architecture

Clean Architecture, four projects, dependencies point inward only:

```
Kitchen.Api (presentation)  →  Kitchen.Application  →  Kitchen.Core (domain, depends on nothing)
Kitchen.Infrastructure  →  implements Kitchen.Core's repository interfaces
```

(Note: the domain project's folder is literally `Kichen.Core` — a long-standing typo — but everything inside it uses the `Kitchen.Core` namespace.)

- **Kitchen.Core** — entities (`ProductDefinition`, `StockItem`), value objects (`ProductName`, `StockItemId`), enums (`UnitType`, `Category`, `StorageLocation`), domain exceptions (under `Exceptions/Catalog`, `Exceptions/Inventory`, `Exceptions/Validation`, all inheriting `KitchenApiException`), and repository interfaces (`IStockItemRepository`, `IProductDefinitionRepository`). All mutation happens through domain methods on the entities (e.g. `StockItem.AdjustAmount`, `PlaceOrMove`; `ProductDefinition.ChangeUnitType`) — these throw the domain exceptions above on invalid input rather than allowing invalid state.
- **Kitchen.Application** — `ICatalogService`/`CatalogService` and `IInventoryService`/`InventoryService` orchestrate; no domain logic, no infrastructure details. Commands (`AddStockItemCommand`, `ModifyProductDefinitionCommand`, etc.) are C# records carrying data from controller to service — CQRS-inspired, not full CQRS (see ADR 0008).
- **Kitchen.Infrastructure** — `KitchenDbContext` + EF configurations, Postgres repository implementations, `DatabaseInitBackgroundService` (applies migrations and seeds sample data on startup if `StockItems` is empty), and the global `ExceptionMiddleware`. The exception middleware deliberately lives here rather than in `Kitchen.Api`, since cross-cutting concerns sit with the rest of the infrastructure (see `docs/architecture.md`).
- **Kitchen.Api** — two controllers (`StockItemsController`, `ProductDefinitionsController`), Request DTOs as records, and five `JsonConverter`s registered globally in `Program.cs` (`UnitTypeConverter`, `CategoryConverter`, `StorageLocationConverter`, `ProductNameConverter`, `StockItemIdConverter`).

### Cross-cutting behaviors worth knowing before changing code

- **`ProductName`** normalizes on construction: trims and lowercases (`"Mleko"` → `"mleko"`). This applies to every read and write across the whole system, not just at the API boundary.
- **`StockItem.Name` has no unique constraint** — the same name can legitimately exist multiple times (e.g. milk in the fridge and milk in the pantry as separate rows). Only `Id` unambiguously identifies a stock item; never assume name uniqueness when writing queries or tests.
- **Auto-linking is bidirectional**: adding a `StockItem` looks up a matching `ProductDefinition` by name and links it (`InventoryService.Add`); adding a `ProductDefinition` walks existing unlinked `StockItem`s with the same name and links them (`CatalogService.LinkToExistingStockItems`). Keep both directions in sync if this logic changes.
- **Enum JSON converters reject unrecognized input** (`400`) rather than silently falling back to `Unspecified` — this was a deliberate fix (see `docs/api.md`, "Allowed enum values"). Don't reintroduce silent fallback behavior. Each converter accepts English name, Polish short form, or `-`/`unspecified`, and always serializes output as the Polish short form.
- **Repository reads use `AsNoTracking()`**; `PostgresStockItemRepository.Add`/`Update` explicitly `Attach()` the linked `Definition` when it's `Detached`, since it typically comes from an untracked read.
- **Error responses** are always `{ "code": "...", "message": "..." }`, where `code` is the exception class name minus `Exception`, in `snake_case` (via `Humanizer.Underscore()`). New domain exceptions should inherit `KitchenApiException` and get mapped in `ExceptionMiddleware`.

### Where to look for more detail

- `docs/architecture.md` — full layer-by-layer walkthrough (entities, EF configurations, DI registration order, `Program.cs` pipeline order).
- `docs/api.md` — exact request/response JSON shapes, full enum alias tables, error code table.
- `docs/runbook.md` — day-to-day ops commands (DB backup/restore).
- `docs/decisions/` — ADRs recording the reasoning behind non-obvious choices (e.g. why POST returns the persisted entity, why enums reject unrecognized input, why commands are CQRS-inspired rather than full CQRS). This directory is gitignored and intentionally not linked from other tracked docs — read it locally for context but don't add links to it from README/docs.

## Conventions

- Conventional Commits, in English, kept concise.
- Versioning is frozen at `0.y.z` until the "Digital Pantry" milestone closes — don't bump to `1.0.0` without being asked.
- Connection strings only ever go through `dotnet user-secrets`, never into a tracked `appsettings*.json`.
