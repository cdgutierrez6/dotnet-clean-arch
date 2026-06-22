## ADDED Requirements

### Requirement: ValidationBehavior con FluentValidation
El pipeline SHALL incluir un `ValidationBehavior<TRequest, TResponse>` que ejecuta todos los `IValidator<TRequest>` registrados para el Command antes de invocar el Handler. Si hay errores de validación, retorna `Result.Failure` con los errores sin llamar al Handler.

#### Scenario: Command inválido rechazado antes del Handler
- **WHEN** se despacha un `CreateUserCommand` con email vacío
- **THEN** `ValidationBehavior` retorna `Result.Failure` con los errores de FluentValidation y el Handler no se ejecuta

#### Scenario: Command válido pasa al Handler
- **WHEN** todos los validators del Command pasan
- **THEN** `ValidationBehavior` llama a `next()` y el Handler se ejecuta normalmente

---

### Requirement: LoggingBehavior con Serilog
El pipeline SHALL incluir un `LoggingBehavior<TRequest, TResponse>` que loguea el nombre del Command/Query, su contenido (sin datos sensibles), y el resultado (success/failure + elapsed ms) usando `ILogger`.

#### Scenario: Log de inicio y fin de operación
- **WHEN** se despacha cualquier Command o Query
- **THEN** el log incluye `[START] CreateUserCommand`, y al finalizar `[END] CreateUserCommand - Succeeded in 45ms` (o `Failed`)

---

### Requirement: TransactionBehavior para Commands
El pipeline SHALL incluir un `TransactionBehavior<TRequest, TResponse>` que abre una transacción EF Core, llama a `next()` (el Handler), y hace commit si `Result.IsSuccess`, o rollback si `Result.IsFailure` o si el Handler lanza excepción. Solo se aplica a Commands (no a Queries).

#### Scenario: Commit en Command exitoso
- **WHEN** el Handler de un Command completa con `Result.Success`
- **THEN** `TransactionBehavior` llama a `UnitOfWork.CommitAsync()` y la transacción se persiste

#### Scenario: Rollback en Command fallido
- **WHEN** el Handler de un Command retorna `Result.Failure`
- **THEN** `TransactionBehavior` hace rollback de la transacción sin persistir cambios parciales

#### Scenario: Queries no abren transacción
- **WHEN** se despacha una Query (implementa `IQuery<T>`)
- **THEN** `TransactionBehavior` detecta que no es un Command y llama a `next()` directamente sin abrir transacción
