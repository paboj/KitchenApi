# Kitchen.Api

A backend system for managing household food supplies, built following **Clean Architecture** and **Domain-Driven Design (DDD)** principles.

## Project Purpose

The Kitchen.Api project provides a reliable tracking system for food inventory. By maintaining a real-time record of ingredients across physical locations—such as the fridge, freezer, or pantry—the system helps users optimize food usage and reduce household waste.

## System Architecture

The application is structured into distinct layers to ensure a clean separation of concerns and high maintainability.

### Layer Responsibilities

*   **Kitchen.Core (Domain):** The heart of the system. It contains business entities (e.g., `Ingredient`), value objects, and domain logic.
*   **Kitchen.Application:** Orchestrates business flow. It uses the **Command Pattern** to translate user intents into actions and manages service-level logic.
*   **Kitchen.Infrastructure:** Handles technical implementation. This layer manages **PostgreSQL** database communication via **Entity Framework Core**.
*   **Kitchen.Api (Presentation):** The entry point of the system. It exposes RESTful endpoints and provides automated documentation via **OpenAPI (Swagger)**.

## Technology Stack

*   **Framework:** .NET
*   **Database:** PostgreSQL
*   **ORM:** Entity Framework Core
*   **DevOps:** Docker
*   **Testing:** xUnit, FluentAssertions, Moq

## Getting Started

### Prerequisites
*   .NET SDK installed.
*   Docker Desktop installed and running.

### 1. Installation and Execution
**Start the Infrastructure:**
Run the following command in the project root to start the database:

`docker-compose up -d`

### 2. Database Setup
The application automatically applies migrations and seeds initial data (Product Catalog) upon startup.

### 3. Run the Application
Execute the project using the .NET CLI:

`dotnet run --project Kitchen.Api`

## Quality Assurance

*   **Domain Validation**: Verification of business rules and state transitions through automated unit tests.
*   **Data Abstraction:** Isolation of persistence logic using the Repository Pattern to simplify maintenance and testing.
*   **Access Control**: Protection of implementation details using internal modifiers and assembly-level visibility attributes.

## Version History

*   **1.0.0 (2026-05-02):** "The Digital Pantry" – Initial stable release with full DDD and Clean Architecture.
*   **0.0.8:** Added PostgreSQL integration and Docker support.
*   **0.0.6:** Transitioned from a monolith to a multi-layered solution.
*   **0.0.4:** Refined the Command pattern and added the xUnit testing suite.
*   **0.0.1:** Initial API setup with basic ingredient tracking.