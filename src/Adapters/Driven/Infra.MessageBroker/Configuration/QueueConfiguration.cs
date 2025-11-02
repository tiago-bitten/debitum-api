namespace Infra.MessageBroker.Configuration;

public sealed class QueueConfiguration
{
    public const string DebtRemindersExchange = "debitum.debt-reminders";
    public const string DebtRemindersQueue = "debitum.debt-reminders.queue";
    public const string DebtRemindersDeadLetterQueue = "debitum.debt-reminders.dlq";
    public const string DebtRemindersRoutingKey = "debt.reminder.send";
}
