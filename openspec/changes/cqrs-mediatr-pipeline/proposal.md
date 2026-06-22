## Why

Los proyectos enterprise .NET necesitan un patrón de separación de responsabilidades que desacople la lógica de negocio de la infraestructura y permita evolucionar cada capa de forma independiente. CQRS con MediatR provee ese desacople: cada operación del sistema es un Command o Query explícito con su Handler aislado, y el pipeline de MediatR permite añadir cross-cutting concerns (validación, logging, transacciones) sin tocar los handlers.

## What Changes

- Commands y Queries como clases inmutables en la capa Application — cada operación del sistema tiene su tipo propio
- Handlers registrados en MediatR que implementan `IRequestHandler<TRequest, TResponse>` — sin dependencias en Infrastructure
- Pipeline Behaviors de MediatR para cross-cutting concerns: `ValidationBehavior` (FluentValidation), `LoggingBehavior` (Serilog), `TransactionBehavior` (EF Core UnitOfWork)
- Interfaces de repositorio definidas en Application, implementadas en Infrastructure — inversión de dependencias estricta
- Result pattern para retornar errores sin excepciones en el happy path

## Capabilities

### New Capabilities

- `cqrs-handlers`: Commands y Queries con sus Handlers en Application layer — separación estricta de escritura y lectura
- `mediatr-pipeline-behaviors`: Pipeline behaviors registrados en MediatR para validación, logging y transacciones como cross-cutting concerns
- `repository-pattern`: Interfaces de repositorio en Domain/Application implementadas en Infrastructure con EF Core

### Modified Capabilities

_(ninguna — implementación inicial del template Clean Architecture)_

## Impact

- **`src/Application/`**: Commands/, Queries/, Behaviors/, Interfaces/ — núcleo de la feature
- **`src/Infrastructure/`**: Repositories/, Persistence/ (DbContext, migrations) — implementaciones
- **`src/API/`**: Controllers delgados que solo despachan al mediator
- **`tests/Unit/`**: Tests de handlers sin dependencias de infraestructura
- **Dependencias NuGet**: `MediatR`, `FluentValidation.DependencyInjectionExtensions`, `Microsoft.EntityFrameworkCore`
