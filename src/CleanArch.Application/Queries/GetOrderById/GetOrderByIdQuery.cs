using CleanArch.Application.Common;
using MediatR;

namespace CleanArch.Application.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderDetailDto>>;

public sealed record OrderDetailDto(
    Guid Id,
    Guid UserId,
    string Status,
    decimal Total,
    string Currency,
    DateTime CreatedAt,
    List<OrderItemDetailDto> Items
);

public sealed record OrderItemDetailDto(
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);
