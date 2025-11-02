using Domain.Shared.Results;

namespace Domain.Debts.Errors;

public static class ReminderSentErrors
{
    public static readonly Error DebtIdRequired = new(
        "ReminderSent.DebtIdRequired",
        "Debt ID is required for creating a reminder sent record",
        ErrorType.Validation);

    public static readonly Error ChannelRequired = new(
        "ReminderSent.ChannelRequired",
        "Channel is required for creating a reminder sent record",
        ErrorType.Validation);
}
