using Application.Debts.Ports;
using Application.Shared.Ports;
using Infra.MessageBroker.Messages;
using Quartz;

namespace API.Jobs;

[DisallowConcurrentExecution]
public class DebtReminderJob(
    IDebtRepository debtRepository,
    IMessagePublisher messagePublisher,
    ILogger<DebtReminderJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Starting DebtReminderJob execution at {Time}", DateTime.UtcNow);

        try
        {
            var debtsNeedingReminder = (await debtRepository.GetDebtsNeedingReminderAsync(
                context.CancellationToken)).ToList();

            logger.LogInformation("Found {Count} debts needing reminder", debtsNeedingReminder.Count);

            foreach (var debt in debtsNeedingReminder)
            {
                var message = new SendDebtReminderMessage
                {
                    DebtId = debt.Id,
                    DebtorId = debt.DebtorId,
                    DebtorName = string.Empty,
                    DebtorPhone = string.Empty,
                    Amount = debt.Amount,
                    DueDate = debt.DueDate,
                    Description = debt.Description
                };

                await messagePublisher.PublishAsync(message, context.CancellationToken);

                logger.LogInformation(
                    "Published reminder message for Debt {DebtId}",
                    debt.Id);
            }

            logger.LogInformation(
                "DebtReminderJob completed successfully. Processed {Count} debts",
                debtsNeedingReminder.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing DebtReminderJob");
            throw;
        }
    }
}
