using CleanArch.Application.Common;
using MediatR;

namespace CleanArch.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid UserId,
    List<OrderItemDto> Items
) : IRequest<Result<Guid>>;

public sealed record OrderItemDto(string ProductName, decimal UnitPrice, int Quantity);
