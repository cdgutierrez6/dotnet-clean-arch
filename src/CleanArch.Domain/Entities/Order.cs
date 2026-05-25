using CleanArch.Domain.Events;
using CleanArch.Domain.ValueObjects;
using CleanArch.Domain.Exceptions;

namespace CleanArch.Domain.Entities;

public sealed class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money Total { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core

    private Order(Guid userId, IEnumerable<OrderItem> items)
    {
        UserId = userId;
        Status = OrderStatus.Pending;
        _items.AddRange(items);
        Total = CalculateTotal();
    }

    public static Order Create(Guid userId, IEnumerable<OrderItem> items)
    {
        var itemsList = items.ToList();

        if (itemsList.Count == 0)
            throw new DomainException("Order must contain at least one item.");

        var order = new Order(userId, itemsList);
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, userId, order.Total.Amount));
        return order;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot confirm an order in {Status} status.");

        Status = OrderStatus.Confirmed;
        SetUpdatedAt();
        AddDomainEvent(new OrderStatusChangedEvent(Id, Status));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new DomainException("Cannot cancel an order that has already been shipped.");

        Status = OrderStatus.Cancelled;
        SetUpdatedAt();
        AddDomainEvent(new OrderStatusChangedEvent(Id, Status));
    }

    private Money CalculateTotal() =>
        Money.Create(_items.Sum(i => i.UnitPrice.Amount * i.Quantity));
}

public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }
