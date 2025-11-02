using Application.Debts.Ports;
using Application.Debts.Services;
using Application.Shared.Ports;
using Infra.MessageBroker.Messages;
using Microsoft.Extensions.Logging;

namespace Infra.MessageBroker.Consumers;

public sealed class SendDebtReminderMessageHandler(
    IDebtRepository debtRepository,
    IDebtorRepository debtorRepository,
    IWhatsAppServiceFactory whatsAppFactory,
    IReminderMessageBuilder messageBuilder,
    IUnitOfWork unitOfWork,
    ILogger<SendDebtReminderMessageHandler> logger)
    : IMessageHandler<SendDebtReminderMessage>
{
    public async Task HandleAsync(SendDebtReminderMessage message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Processing reminder for Debt {DebtId}",
            message.DebtId);

        try
        {
            await unitOfWork.BeginAsync(cancellationToken);

            var debt = await debtRepository.GetByIdAsync(message.DebtId, cancellationToken);
            if (debt is null)
            {
                logger.LogWarning("Debt {DebtId} not found", message.DebtId);
                return;
            }

            if (debt.IsPaid)
            {
                logger.LogInformation("Debt {DebtId} is already paid, skipping reminder", message.DebtId);
                return;
            }

            var debtor = await debtorRepository.GetByIdAsync(message.DebtorId, cancellationToken);
            if (debtor is null)
            {
                logger.LogWarning("Debtor {DebtorId} not found", message.DebtorId);
                return;
            }

            var reminderMessage = messageBuilder.BuildMessage(debtor, debt);
            var whatsAppService = whatsAppFactory.Create();

            var success = await whatsAppService.SendTextMessageAsync(
                instanceName: "debitum-instance",
                recipientNumber: debtor.Phone.Number,
                message: reminderMessage,
                cancellationToken: cancellationToken);

            if (!success)
            {
                logger.LogError(
                    "Failed to send WhatsApp message for Debt {DebtId}",
                    message.DebtId);
                throw new InvalidOperationException($"Failed to send WhatsApp message for Debt {message.DebtId}");
            }

            var recordResult = debt.RecordReminderSent("whatsapp", reminderMessage);
            if (recordResult.IsFailure)
            {
                logger.LogError(
                    "Failed to record reminder sent for Debt {DebtId}: {Error}",
                    message.DebtId,
                    recordResult.Error);
                throw new InvalidOperationException($"Failed to record reminder: {recordResult.Error}");
            }

            await debtRepository.UpdateAsync(debt);
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Successfully sent reminder for Debt {DebtId} to {DebtorPhone}",
                message.DebtId,
                debtor.Phone.Number);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            logger.LogError(
                ex,
                "Error processing reminder for Debt {DebtId}",
                message.DebtId);
            throw;
        }
    }
}
