## ADDED Requirements

### Requirement: Commands como clases inmutables en Application
Cada operación de escritura SHALL ser un record inmutable que implementa `IRequest<Result<TResponse>>` en `src/Application/Commands/<Feature>/`. El Command no contiene lógica de negocio — solo los datos de entrada.

#### Scenario: Command dispatched via mediator
- **WHEN** el controller llama a `_mediator.Send(new CreateUserCommand(name, email))`
- **THEN** MediatR resuelve y ejecuta el `CreateUserCommandHandler` registrado para ese Command sin que el controller conozca el handler

#### Scenario: Query dispatched sin efecto de escritura
- **WHEN** el controller llama a `_mediator.Send(new GetUserByIdQuery(id))`
- **THEN** el handler correspondiente retorna `Result<UserDto>` sin modificar el estado del sistema

---

### Requirement: Handlers en Application sin referencia a Infrastructure
Los Handlers SHALL implementar `IRequestHandler<TCommand, Result<TResponse>>` y depender únicamente de interfaces definidas en Application o Domain — nunca de clases concretas de Infrastructure (DbContext, EF Core, etc.).

#### Scenario: Handler testeable sin EF Core
- **WHEN** se ejecuta el test unitario de `CreateUserCommandHandler`
- **THEN** el test pasa usando mocks de `IUserRepository` e `IUnitOfWork` sin levantar base de datos

#### Scenario: Handler registrado automáticamente
- **WHEN** la aplicación arranca
- **THEN** MediatR escanea el assembly de Application y registra todos los handlers automáticamente vía `AddMediatR(typeof(ApplicationAssemblyMarker))`

---

### Requirement: Result pattern para errores de dominio
Los Handlers SHALL retornar `Result<T>` en lugar de lanzar excepciones para errores esperados. `Result<T>` expone `IsSuccess`, `Error` (tipo+mensaje), y `Value` (solo si IsSuccess).

#### Scenario: Handler retorna error NotFound
- **WHEN** el handler busca una entidad por ID y no existe
- **THEN** retorna `Result.Failure<UserDto>(Error.NotFound("User.NotFound", "Usuario no encontrado"))` sin lanzar excepción

#### Scenario: Controller mapea Result a HTTP status
- **WHEN** el handler retorna `Result.Failure` con tipo `NotFound`
- **THEN** el controller retorna HTTP 404 con el mensaje de error; si `IsSuccess`, retorna HTTP 200/201 con el Value
