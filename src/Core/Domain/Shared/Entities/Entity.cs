using Domain.Shared.Events;

namespace Domain.Shared.Entities;

public abstract class Entity
{
    private readonly List<IEvent> _events = [];

    public string Id { get; protected set; } = string.Empty;
    public string PublicId { get; protected set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; protected set; }

    public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();

    public void Raise(IEvent @event) => _events.Add(@event);
    public void ClearEvents() => _events.Clear();
}