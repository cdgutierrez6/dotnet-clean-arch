using CleanArch.Domain.ValueObjects;
using CleanArch.Domain.Exceptions;

namespace CleanArch.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { } // EF Core

    public static OrderItem Create(string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        return new OrderItem
        {
            ProductName = productName,
            UnitPrice = Money.Create(unitPrice),
            Quantity = quantity,
        };
    }
}
