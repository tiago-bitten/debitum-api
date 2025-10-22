using Domain.Shared.Entities;
using Domain.Shared.ValueObjects;

namespace Domain.Debtors.Entities;

public sealed class Debtor : EntityDeletable
{
    public string Name { get; private set; } = string.Empty;
    public Phone Phone { get; private set; } = null!;
    public CustomerDebtor Customer { get; private set; } = null!;
}

public sealed class CustomerDebtor
{
    public string Id { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
}