namespace Domain.Shared.Entities;

public abstract class Entity
{
    public string Id { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
}