## ADDED Requirements

### Requirement: IRepository definido en Application
La interfaz `IRepository<T>` SHALL estar definida en `src/Application/Interfaces/` con métodos: `GetByIdAsync(id)`, `GetAllAsync()`, `AddAsync(entity)`, `UpdateAsync(entity)`, `DeleteAsync(entity)`. La implementación concreta `Repository<T>` vive en Infrastructure.

#### Scenario: Handler usa IRepository sin conocer EF Core
- **WHEN** `GetUserByIdQueryHandler` llama a `_userRepository.GetByIdAsync(id)`
- **THEN** el handler no tiene referencia a `DbContext`, `DbSet` ni ningún tipo de EF Core

#### Scenario: Test unitario mockea IRepository
- **WHEN** el test unitario configura `_mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user)`
- **THEN** el handler se ejecuta sin levantar base de datos y el test es determinista

---

### Requirement: IUnitOfWork en Application
La interfaz `IUnitOfWork` SHALL definir `CommitAsync(CancellationToken)` en `src/Application/Interfaces/`. La implementación `UnitOfWork` en Infrastructure envuelve el `SaveChangesAsync()` de EF Core.

#### Scenario: TransactionBehavior usa IUnitOfWork
- **WHEN** el Handler completa con éxito
- **THEN** `TransactionBehavior` llama a `_unitOfWork.CommitAsync()` sin conocer que internamente es `DbContext.SaveChangesAsync()`

---

### Requirement: Repositorios específicos extienden IRepository
Los repositorios de entidades SHALL tener interfaces específicas (ej: `IUserRepository : IRepository<User>`) cuando necesitan métodos adicionales (ej: `GetByEmailAsync`). Estos métodos adicionales se definen en la interfaz específica, no en `IRepository<T>`.

#### Scenario: Método específico del repositorio
- **WHEN** el Handler llama a `_userRepository.GetByEmailAsync(email)`
- **THEN** el método existe en `IUserRepository` (no en `IRepository<T>`) y la implementación en Infrastructure lo resuelve contra EF Core
