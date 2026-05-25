# .NET 8 Clean Architecture Template — Enterprise Ready

[![.NET](https://img.shields.io/badge/.NET_8-5C2D91?style=flat-square&logo=.net&logoColor=white)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23_12-239120?style=flat-square&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp)
[![EF Core](https://img.shields.io/badge/EF_Core_8-512BD4?style=flat-square&logo=.net&logoColor=white)](https://docs.microsoft.com/en-us/ef/core)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat-square&logo=docker&logoColor=white)](https://www.docker.com)
[![xUnit](https://img.shields.io/badge/xUnit-5D4F85?style=flat-square)](https://xunit.net)

Template de referencia para APIs enterprise en **.NET 8** con Clean Architecture, CQRS, MediatR y EF Core. Basado en patrones implementados en sistemas de gestión financiera y telemetría vehicular en producción.

---

## Capas de la Arquitectura

```
┌────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                         │
│              WebApi (Controllers, Middlewares)                │
└────────────────────────┬───────────────────────────────────────┘
                         │ DTOs / Requests / Responses
┌────────────────────────▼───────────────────────────────────────┐
│                    APPLICATION LAYER                          │
│    Commands / Queries (CQRS) · Handlers · Validators         │
│    MediatR · FluentValidation · AutoMapper                   │
└────────────────────────┬───────────────────────────────────────┘
                         │ Interfaces / Domain Models
┌────────────────────────▼───────────────────────────────────────┐
│                     DOMAIN LAYER                              │
│    Entities · Value Objects · Domain Events                  │
│    Aggregates · Repository Interfaces · Business Rules       │
└────────────────────────┬───────────────────────────────────────┘
                         │ Implementations
┌────────────────────────▼───────────────────────────────────────┐
│                  INFRASTRUCTURE LAYER                         │
│    EF Core · Repositories · External Services               │
│    Email · File Storage · Cache (Redis)                      │
└────────────────────────────────────────────────────────────────┘
```

---

## Estructura del Proyecto

```
dotnet-clean-arch/
├── src/
│   ├── CleanArch.Domain/              ← Núcleo de negocio (sin dependencias)
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── User.cs
│   │   │   └── Order.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   └── Money.cs
│   │   ├── Events/
│   │   │   ├── OrderCreatedEvent.cs
│   │   │   └── UserRegisteredEvent.cs
│   │   ├── Repositories/              ← Interfaces (no implementaciones)
│   │   │   ├── IUserRepository.cs
│   │   │   └── IOrderRepository.cs
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       └── NotFoundException.cs
│   │
│   ├── CleanArch.Application/         ← Casos de uso (CQRS)
│   │   ├── Commands/
│   │   │   ├── CreateOrder/
│   │   │   │   ├── CreateOrderCommand.cs
│   │   │   │   ├── CreateOrderHandler.cs
│   │   │   │   └── CreateOrderValidator.cs
│   │   │   └── RegisterUser/
│   │   │       ├── RegisterUserCommand.cs
│   │   │       ├── RegisterUserHandler.cs
│   │   │       └── RegisterUserValidator.cs
│   │   ├── Queries/
│   │   │   ├── GetOrderById/
│   │   │   │   ├── GetOrderByIdQuery.cs
│   │   │   │   └── GetOrderByIdHandler.cs
│   │   │   └── GetUserOrders/
│   │   ├── DTOs/
│   │   ├── Mappings/
│   │   └── Behaviors/
│   │       ├── ValidationBehavior.cs  ← Pipeline MediatR
│   │       └── LoggingBehavior.cs
│   │
│   ├── CleanArch.Infrastructure/      ← Implementaciones técnicas
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── UserRepository.cs
│   │   │   │   └── OrderRepository.cs
│   │   │   └── Migrations/
│   │   ├── Services/
│   │   │   ├── EmailService.cs
│   │   │   └── CacheService.cs
│   │   └── DependencyInjection.cs
│   │
│   └── CleanArch.WebApi/              ← Capa de presentación
│       ├── Controllers/
│       │   ├── OrdersController.cs
│       │   └── UsersController.cs
│       ├── Middleware/
│       │   ├── ErrorHandlingMiddleware.cs
│       │   └── RequestLoggingMiddleware.cs
│       ├── Program.cs
│       └── appsettings.json
│
└── tests/
    ├── CleanArch.Domain.Tests/
    ├── CleanArch.Application.Tests/
    └── CleanArch.Integration.Tests/
```

---

## Inicio Rápido

### Prerrequisitos
- .NET 8 SDK
- Docker Desktop

### Levantar con Docker

```bash
git clone https://github.com/cdgutierrez6/dotnet-clean-arch.git
cd dotnet-clean-arch

docker-compose up -d

# La API estará disponible en:
# https://localhost:7001
# http://localhost:5001
# Swagger UI: https://localhost:7001/swagger
```

### Sin Docker

```bash
# Levantar PostgreSQL
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=dev123 postgres:15

# Aplicar migraciones
dotnet ef database update --project src/CleanArch.Infrastructure --startup-project src/CleanArch.WebApi

# Correr la API
dotnet run --project src/CleanArch.WebApi
```

---

## Patrones Implementados

| Patrón | Librería | Propósito |
|---|---|---|
| **CQRS** | MediatR | Separar lecturas de escrituras |
| **Pipeline Behaviors** | MediatR | Validación y logging transversales |
| **Repository** | EF Core | Abstracción de persistencia |
| **Unit of Work** | EF Core | Transacciones atómicas |
| **Domain Events** | MediatR | Desacoplamiento de efectos secundarios |
| **Specification** | Custom | Consultas complejas reutilizables |
| **Result Pattern** | Custom | Manejo de errores sin excepciones |
| **Value Objects** | Domain | Inmutabilidad y validación |

---

## Ejemplo CQRS

```csharp
// Command
public record CreateOrderCommand(
    Guid UserId,
    List<OrderItemDto> Items
) : IRequest<Result<Guid>>;

// Handler
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure<Guid>(UserErrors.NotFound);

        var order = Order.Create(user.Id, request.Items.Select(MapToDomain));

        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Domain events se publican automáticamente
        return Result.Success(order.Id);
    }
}
```

---

## Tecnologías

- **.NET 8** + **C# 12**
- **MediatR** (CQRS + Pipeline Behaviors)
- **Entity Framework Core 8**
- **FluentValidation**
- **AutoMapper**
- **Serilog** (structured logging)
- **PostgreSQL** (producción) / **SQLite** (tests)
- **Redis** (cache)
- **xUnit** + **FluentAssertions** + **Moq** (testing)
- **Swagger / OpenAPI**

---

## Tests

```bash
# Correr todos los tests
dotnet test

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## Contexto de Producción

Esta arquitectura fue aplicada en sistemas de gestión de pagos y procesamiento de transacciones en **INGENEO** y **DOCTUS** (2019–2022), donde la separación de responsabilidades y la testabilidad eran críticas para el cumplimiento regulatorio.

---

## Autor

**Cristian Daniel Gutiérrez S.** — Solutions Architect | Senior .NET Engineer

[LinkedIn](https://www.linkedin.com/in/cristian-daniel-guti%C3%A9rrez-segura) · [Portfolio](https://portafolio-frontend-wheat.vercel.app) · [cdgutierrez6@gmail.com](mailto:cdgutierrez6@gmail.com)
