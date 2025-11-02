namespace Infra.MessageBroker.Messages;

public record SendDebtReminderMessage
{
    public Guid DebtId { get; init; }
    public Guid DebtorId { get; init; }
    public string DebtorName { get; init; } = string.Empty;
    public string DebtorPhone { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime DueDate { get; init; }
    public string? Description { get; init; }
}
