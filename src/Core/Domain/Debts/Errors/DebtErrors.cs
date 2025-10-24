using Domain.Shared.Results;

namespace Domain.Debts.Errors;

public static class DebtErrors
{
    public static readonly Error AmountInvalid =
        Error.Problem("Debt.Amount", "Debt amount must be greater than zero.");

    public static readonly Error DueDateInvalid =
        Error.Problem("Debt.DueDate", "Due date must be in the future.");

    public static readonly Error NotFound =
        Error.NotFound("Debt.NotFound", "Debt not found.");

    public static readonly Error AlreadyPaid =
        Error.Problem("Debt.AlreadyPaid", "This debt has already been paid.");

    public static readonly Error ErrorSendingMessage =
        Error.Problem("Debt.ErrorSendingMessage", "Error sending message.");
}