using CleanArch.Domain.Entities;

namespace CleanArch.Domain.Events;

public sealed record OrderStatusChangedEvent(Guid OrderId, OrderStatus NewStatus) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
