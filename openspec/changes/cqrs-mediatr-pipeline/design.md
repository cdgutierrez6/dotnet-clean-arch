## Context

El template Clean Architecture para .NET 8 necesita demostrar cómo implementar CQRS sin acoplamiento entre capas. El problema raíz de muchos proyectos enterprise es que los controllers llaman directamente a servicios que llaman directamente a repositorios — toda la lógica mezclada, difícil de testear y de evolucionar. MediatR resuelve esto siendo el único punto de entrada desde la API hacia la lógica de negocio.

## Goals / Non-Goals

**Goals:**
- Controllers delgados que solo despachan al IMediator — sin lógica de negocio en la API layer
- Handlers completamente testeables sin levantar la aplicación (sin dependencias de EF Core o HTTP)
- Pipeline behaviors como decoradores reutilizables para validation, logging y transactions
- Inversión de dependencias estricta: Application no referencia Infrastructure

**Non-Goals:**
- Read models separados o proyecciones (CQRS completo con DB de lectura separada — fuera de scope del template)
- Event sourcing
- Autenticación/autorización (concern separado, no parte del pipeline CQRS)

## Decisions

### D1 — MediatR como mediador único sobre llamadas directas a servicios

**Decisión**: toda operación del sistema pasa por `IMediator.Send()` — los controllers no conocen ningún servicio directamente.

**Alternativas consideradas**:
- _Servicios de aplicación directos_: `IUserService`, `IOrderService` inyectados en controllers — rompe el principio de responsabilidad única cuando un caso de uso involucra múltiples servicios.
- _Command Bus propio_: implementar el patrón desde cero — reimplementar lo que MediatR ya resuelve bien.

**Rationale**: MediatR provee el registro automático de handlers, el pipeline de behaviors, y el desacople entre caller y handler. Un controller no necesita saber qué handler procesa su request — solo necesita el contrato (Command/Query + Response).

---

### D2 — Pipeline Behaviors en orden: Logging → Validation → Transaction

**Decisión**: registrar behaviors en este orden estricto. MediatR los ejecuta como middleware en orden de registro.

**Rationale**:
- `LoggingBehavior` primero: captura el inicio y fin de TODA operación, incluyendo las que fallan en validación
- `ValidationBehavior` segundo: rechaza requests inválidos antes de abrir una transacción
- `TransactionBehavior` tercero: solo Commands abren transacción (Queries no necesitan UoW)

---

### D3 — Result pattern sobre excepciones para errores de dominio

**Decisión**: los Handlers retornan `Result<T>` (con `IsSuccess`, `Error`, `Value`) en lugar de lanzar excepciones para errores esperados (NotFound, Conflict, Validation).

**Alternativas consideradas**:
- _Excepciones de dominio_: `NotFoundException`, `ConflictException` — atraparlas en middleware. Problema: el flujo de control por excepciones es caro y oscurece la intención.

**Rationale**: `Result<T>` hace explícito en la firma del Handler que puede fallar. El controller mapea el Result a HTTP status code. Las excepciones quedan para errores inesperados de infraestructura.

---

### D4 — Repositorios genéricos en Infrastructure, interfaces en Application

**Decisión**: `IRepository<T>` y `IUnitOfWork` definidos en Application layer; `Repository<T>` y `UnitOfWork` implementados en Infrastructure.

**Rationale**: Application puede ser compilado y testeado sin referenciar Infrastructure. Los Handlers hacen mock de las interfaces de repositorio en tests unitarios sin EF Core.

## Risks / Trade-offs

| Riesgo | Mitigación |
|--------|-----------|
| Proliferación de clases (un archivo por Command/Query) | Convención de carpetas: `Commands/<Feature>/`, `Queries/<Feature>/` — navegación consistente |
| Overhead de MediatR en operaciones de alta frecuencia | MediatR usa reflection en el primer registro; requests subsiguientes son O(1) por caché de tipo |
| TransactionBehavior abre transacción en todos los Commands aunque no siempre se necesite | Atributo `[Transactional]` opcional en Commands para opt-in explícito (fase 2) |

## Migration Plan

Este es un template — no hay migration. El onboarding para un proyecto nuevo:
1. Clonar el repo
2. Renombrar `CleanArch` namespace al nombre del proyecto
3. Aplicar EF Core migration inicial: `dotnet ef database update`
4. Arrancar: `dotnet run --project src/CleanArch.API`

## Open Questions

- ¿Incluir `IReadRepository<T>` separado de `IRepository<T>` para Queries (sin métodos de escritura)?
- ¿Result pattern propio o depender de `FluentResults` / `ErrorOr` NuGet?
