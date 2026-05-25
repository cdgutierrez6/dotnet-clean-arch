using CleanArch.Application.Common;
using CleanArch.Domain.Repositories;
using MediatR;

namespace CleanArch.Application.Queries.GetOrderById;

public sealed class GetOrderByIdHandler(
    IOrderRepository orderRepository
) : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
{
    public async Task<Result<OrderDetailDto>> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, ct);

        if (order is null)
            return Result.Failure<OrderDetailDto>($"Order '{request.OrderId}' not found.");

        var dto = new OrderDetailDto(
            order.Id,
            order.UserId,
            order.Status.ToString(),
            order.Total.Amount,
            order.Total.Currency,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDetailDto(
                i.ProductName,
                i.UnitPrice.Amount,
                i.Quantity,
                i.UnitPrice.Amount * i.Quantity
            )).ToList()
        );

        return Result.Success(dto);
    }
}
