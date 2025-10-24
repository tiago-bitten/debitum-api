using Application.Shared.Messaging;

namespace Application.Debts.Features.RegisterDebtor;

public sealed record RegisterDebtorCommand(
    Guid CustomerId,
    string Name,
    string Phone,
    string? Email
) : ICommand<Guid>;