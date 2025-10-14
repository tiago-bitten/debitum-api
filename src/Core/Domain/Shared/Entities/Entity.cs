namespace Domain.Shared.Entities;

public abstract class Entity
{
    public string Id { get; protected set; } = string.Empty;
    public string PublicId { get; protected set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; protected set; }
}