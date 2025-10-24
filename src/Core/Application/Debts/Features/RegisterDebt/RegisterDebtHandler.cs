using Application.Debts.Ports;
using Application.Shared.Messaging;
using Domain.Debts.Errors;
using Domain.Shared.Results;

namespace Application.Debts.Features.RegisterDebt;

internal sealed class RegisterDebtHandler(
    IDebtorRepository debtorRepository)
    : ICommandHandler<RegisterDebtCommand>
{
    public async Task<Result> HandleAsync(
        RegisterDebtCommand command,
        CancellationToken cancellationToken = default)
    {
        var debtor = await debtorRepository.GetByIdAsync(command.DebtorId, cancellationToken);
        if (debtor is null)
            return DebtorErrors.NotFound;

        var addDebtResult = debtor.AddDebt(command.Amount, command.DueDate, command.Description);
        if (addDebtResult.IsFailure)
            return addDebtResult.Error;

        await debtorRepository.UpdateAsync(debtor);

        return Result.Ok();
    }
}