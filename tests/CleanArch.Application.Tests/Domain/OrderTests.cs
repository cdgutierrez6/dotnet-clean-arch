using CleanArch.Domain.Entities;
using CleanArch.Domain.Events;
using CleanArch.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CleanArch.Application.Tests.Domain;

public sealed class OrderTests
{
    private readonly Guid _userId = Guid.NewGuid();

    private static IEnumerable<OrderItem> ValidItems() =>
    [
        OrderItem.Create("Product A", 100m, 2),
        OrderItem.Create("Product B", 50m, 1),
    ];

    [Fact]
    public void Create_WithValidItems_ShouldCalculateTotalCorrectly()
    {
        var order = Order.Create(_userId, ValidItems());

        order.Total.Amount.Should().Be(250m); // (100*2) + (50*1)
        order.Total.Currency.Should().Be("USD");
        order.Status.Should().Be(OrderStatus.Pending);
        order.UserId.Should().Be(_userId);
    }

    [Fact]
    public void Create_ShouldRaiseOrderCreatedEvent()
    {
        var order = Order.Create(_userId, ValidItems());

        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderCreatedEvent>();

        var @event = (OrderCreatedEvent)order.DomainEvents[0];
        @event.UserId.Should().Be(_userId);
        @event.Total.Should().Be(250m);
    }

    [Fact]
    public void Create_WithNoItems_ShouldThrowDomainException()
    {
        var act = () => Order.Create(_userId, []);

        act.Should().Throw<DomainException>()
            .WithMessage("*at least one item*");
    }

    [Fact]
    public void Confirm_FromPending_ShouldChangeStatusToConfirmed()
    {
        var order = Order.Create(_userId, ValidItems());
        order.ClearDomainEvents();

        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderStatusChangedEvent>();
    }

    [Fact]
    public void Confirm_FromConfirmed_ShouldThrowDomainException()
    {
        var order = Order.Create(_userId, ValidItems());
        order.Confirm();

        var act = () => order.Confirm();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_FromPending_ShouldChangeStatusToCancelled()
    {
        var order = Order.Create(_userId, ValidItems());

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}
