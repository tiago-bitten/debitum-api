using Application.Shared.Messaging;

namespace Application.Debts.Features.SendReminder;

public sealed record SendReminderCommand(
    Guid DebtId,
    string? CustomMessage = null
) : ICommand;