namespace CleanArch.Domain.Events;

public sealed record OrderCreatedEvent(Guid OrderId, Guid UserId, decimal Total) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
