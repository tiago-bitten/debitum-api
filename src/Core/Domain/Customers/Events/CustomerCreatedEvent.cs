using Domain.Shared.Events;

namespace Domain.Customers.Events;

public sealed record CustomerCreatedEvent(string Id) : IEvent;