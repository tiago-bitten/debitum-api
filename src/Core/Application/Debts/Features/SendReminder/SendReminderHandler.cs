using Application.Debts.Ports;
using Application.Debts.Services;
using Application.Shared.Messaging;
using Application.Shared.Ports;
using Domain.Debts.Errors;
using Domain.Shared.Results;

namespace Application.Debts.Features.SendReminder;

internal sealed class SendReminderHandler(
    IDebtRepository debtRepository,
    IDebtorRepository debtorRepository,
    IWhatsAppServiceFactory whatsAppFactory,
    IReminderMessageBuilder messageBuilder)
    : ICommandHandler<SendReminderCommand>
{
    public async Task<Result> HandleAsync(
        SendReminderCommand command,
        CancellationToken cancellationToken = default)
    {
        var debt = await debtRepository.GetByIdAsync(command.DebtId, cancellationToken);
        if (debt is null)
            return DebtErrors.NotFound;

        var debtor = await debtorRepository.GetByIdAsync(debt.DebtorId, cancellationToken);
        if (debtor is null)
            return DebtorErrors.NotFound;

        var message = command.CustomMessage ?? messageBuilder.BuildMessage(debtor, debt);

        var whatsAppService = whatsAppFactory.Create();

        var success = await whatsAppService.SendTextMessageAsync(
            instanceName: "debitum-instance",
            recipientNumber: debtor.Phone.Number,
            message: message,
            cancellationToken: cancellationToken);

        if (!success)
        {
            return DebtErrors.ErrorSendingMessage;
        }

        var recordResult = debt.RecordReminderSent();
        if (recordResult.IsFailure)
            return recordResult.Error;

        await debtRepository.UpdateAsync(debt);

        return Result.Ok();
    }
}