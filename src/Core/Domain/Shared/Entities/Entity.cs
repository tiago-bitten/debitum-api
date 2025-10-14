using System.Security.Cryptography;
using System.Text;
using Domain.Shared.Events;

namespace Domain.Shared.Entities;

public abstract class Entity
{
    private readonly List<IEvent> _events = [];

    protected Entity()
    {
        PublicId = GenerateId();
    }

    public string Id { get; protected init; } = string.Empty;
    public string PublicId { get; protected init; }
    public DateTimeOffset CreatedAt { get; protected init; } = DateTimeOffset.UtcNow;

    public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();

    public void Raise(IEvent @event) => _events.Add(@event);
    public void ClearEvents() => _events.Clear();

    private string GenerateId()
    {
        var entityName = GetType().Name.ToLower();

        var uniqueSeed = Guid.CreateVersion7().ToString("N");
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(uniqueSeed));

        var uniquePart = Convert.ToHexStringLower(hashBytes, 0, 5);

        return $"{entityName}_{uniquePart}";
    }
}