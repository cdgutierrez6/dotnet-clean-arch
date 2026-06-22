## 1. Estructura de Capas y Dependencias

- [ ] 1.1 Crear `src/CleanArch.Domain/` con carpetas `Entities/`, `ValueObjects/`, `Interfaces/` — proyecto de clase sin dependencias externas
- [ ] 1.2 Crear `src/CleanArch.Application/` con carpetas `Commands/`, `Queries/`, `Behaviors/`, `Interfaces/`, `DTOs/` — solo referencias a Domain y MediatR/FluentValidation
- [ ] 1.3 Crear `src/CleanArch.Infrastructure/` con `Persistence/` (DbContext, migrations, repos) — referencia a Application
- [ ] 1.4 Crear `src/CleanArch.API/` con `Controllers/`, `Middleware/`, `Extensions/` — referencia a Application y registra Infrastructure en DI
- [ ] 1.5 Agregar NuGet packages: `MediatR` + `MediatR.Extensions.Microsoft.DependencyInjection` en Application; `FluentValidation.DependencyInjectionExtensions` en Application; `Microsoft.EntityFrameworkCore.SqlServer` + `Microsoft.EntityFrameworkCore.Tools` en Infrastructure

## 2. Result Pattern

- [ ] 2.1 Implementar `Result<T>` y `Result` (sin valor) en `Domain/Common/` con `IsSuccess`, `Error`, `Value`, métodos estáticos `Success(value)` y `Failure(error)`
- [ ] 2.2 Implementar `Error` record con `Code` (string) y `Description` (string), y constantes estáticas de errores comunes: `Error.NotFound`, `Error.Validation`, `Error.Conflict`
- [ ] 2.3 Implementar `ResultExtensions` en API layer con `ToActionResult()` que mapea Result a IActionResult (200/201/400/404/409)

## 3. Commands, Queries y Handlers de Ejemplo

- [ ] 3.1 Implementar `CreateUserCommand` (record con Name, Email) + `CreateUserCommandHandler` que usa `IUserRepository` y `IUnitOfWork`, retorna `Result<Guid>`
- [ ] 3.2 Implementar `GetUserByIdQuery` (record con Id) + `GetUserByIdQueryHandler` que usa `IUserRepository`, retorna `Result<UserDto>`
- [ ] 3.3 Implementar `CreateUserCommandValidator : AbstractValidator<CreateUserCommand>` con reglas: Name no vacío, Email formato válido
- [ ] 3.4 Implementar `IUserRepository : IRepository<User>` en Application con `GetByEmailAsync(email)`; implementar `UserRepository` en Infrastructure

## 4. MediatR Pipeline Behaviors

- [ ] 4.1 Implementar `ValidationBehavior<TRequest, TResponse>` que ejecuta todos los `IValidator<TRequest>` antes de `next()`, retorna `Result.Failure` con errores de FluentValidation si hay fallos
- [ ] 4.2 Implementar `LoggingBehavior<TRequest, TResponse>` que loguea nombre del request, start time, y result (success/failure + elapsed ms) con `ILogger`
- [ ] 4.3 Implementar `TransactionBehavior<TRequest, TResponse>` que abre transacción EF Core, llama `next()`, hace commit en success o rollback en failure — solo actúa en Commands (interfaz marker `ICommand`)
- [ ] 4.4 Registrar behaviors en orden en `DependencyInjection.cs` de Application: `LoggingBehavior` → `ValidationBehavior` → `TransactionBehavior`

## 5. Infrastructure y Persistencia

- [ ] 5.1 Implementar `AppDbContext : DbContext` en Infrastructure con `DbSet<User>` y configuración de entidades via `IEntityTypeConfiguration`
- [ ] 5.2 Implementar `Repository<T>` genérico y `UnitOfWork` (wrappea `AppDbContext.SaveChangesAsync`)
- [ ] 5.3 Registrar toda la infraestructura en `AddInfrastructure(IServiceCollection, IConfiguration)` extension method
- [ ] 5.4 Crear migration inicial: `dotnet ef migrations add InitialCreate --project src/CleanArch.Infrastructure --startup-project src/CleanArch.API`

## 6. API Controllers

- [ ] 6.1 Implementar `UsersController` con `POST /api/users` (dispatcha `CreateUserCommand`) y `GET /api/users/{id}` (dispatcha `GetUserByIdQuery`) — controllers sin lógica, solo `_mediator.Send()` + `result.ToActionResult()`
- [ ] 6.2 Implementar `GlobalExceptionMiddleware` que captura excepciones no manejadas y retorna HTTP 500 con Problem Details

## 7. Tests

- [ ] 7.1 Test unitario `CreateUserCommandHandlerTests`: mock de `IUserRepository` e `IUnitOfWork`, verifica que handler retorna `Result.Success` y llama a `AddAsync` + `CommitAsync`
- [ ] 7.2 Test unitario `ValidationBehaviorTests`: verifica que comando inválido retorna `Result.Failure` sin llamar al handler
- [ ] 7.3 Test de integración con `WebApplicationFactory`: `POST /api/users` con payload válido retorna HTTP 201 con el Guid del usuario creado
