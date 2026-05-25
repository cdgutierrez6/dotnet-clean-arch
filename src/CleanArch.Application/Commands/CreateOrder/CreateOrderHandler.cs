using CleanArch.Application.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Exceptions;
using CleanArch.Domain.Repositories;
using MediatR;

namespace CleanArch.Application.Commands.CreateOrder;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result.Failure<Guid>($"User '{request.UserId}' not found.");

        try
        {
            var items = request.Items
                .Select(i => OrderItem.Create(i.ProductName, i.UnitPrice, i.Quantity));

            var order = Order.Create(request.UserId, items);

            await orderRepository.AddAsync(order, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result.Success(order.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
