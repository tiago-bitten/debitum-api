using Domain.Shared.Results;

namespace Domain.Debts.Errors;

public static class DebtorErrors
{
    public static readonly Error NameRequired =
        Error.Problem("Debtor.Name", "Debtor name is required.");

    public static readonly Error NotFound =
        Error.NotFound("Debtor.NotFound", "Debtor not found.");
}