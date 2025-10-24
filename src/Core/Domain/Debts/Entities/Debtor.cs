using Domain.Debts.Errors;
using Domain.Shared.Entities;
using Domain.Shared.Results;
using Domain.Shared.ValueObjects;

namespace Domain.Debts.Entities;

public sealed class Debtor : EntityDeletable
{
    private readonly List<Debt> _debts = [];

    private Debtor()
    {
    }

    private Debtor(Guid customerId, string name, Phone phone, Email? email)
        => (CustomerId, Name, Phone, Email) = (customerId, name, phone, email);

    public Guid CustomerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Phone Phone { get; private set; } = null!;
    public Email? Email { get; private set; }

    public IReadOnlyCollection<Debt> Debts => _debts.AsReadOnly();

    public static Result<Debtor> Create(Guid customerId, string name, Phone phone, Email? email)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DebtorErrors.NameRequired;

        return new Debtor(customerId, name, phone, email);
    }

    public Result AddDebt(decimal amount, DateTime dueDate, string? description)
    {
        var debtResult = Debt.Create(
            amount,
            dueDate,
            description);

        if (debtResult.IsFailure)
            return debtResult.Error;

        var debt = debtResult.Value;

        _debts.Add(debt);

        return Result.Ok();
    }
}