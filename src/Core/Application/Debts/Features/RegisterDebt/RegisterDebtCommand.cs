using Application.Shared.Messaging;

namespace Application.Debts.Features.RegisterDebt;

public sealed record RegisterDebtCommand(
    Guid DebtorId,
    decimal Amount,
    DateTime DueDate,
    string? Description
) : ICommand;