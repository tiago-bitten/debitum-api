using Domain.Debts.Errors;
using Domain.Shared.Entities;
using Domain.Shared.Results;

namespace Domain.Debts.Entities;

public sealed class Debt : Entity
{
    private Debt()
    {
    }

    private Debt(decimal amount, DateTime dueDate, string? description)
        => (Amount, DueDate, Description, IsPaid) = (amount, dueDate, description, false);

    public Guid DebtorId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }
    public string? Description { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? LastReminderSentAt { get; private set; }
    public int RemindersSentCount { get; private set; }

    internal static Result<Debt> Create(decimal amount, DateTime dueDate, string? description = null)
    {
        if (amount <= 0)
            return DebtErrors.AmountInvalid;

        if (dueDate < DateTime.UtcNow.Date)
            return DebtErrors.DueDateInvalid;

        return new Debt(amount, dueDate, description);
    }

    internal Result MarkAsPaid()
    {
        if (IsPaid)
            return DebtErrors.AlreadyPaid;

        IsPaid = true;
        PaidAt = DateTime.UtcNow;

        return Result.Ok();
    }

    public Result RecordReminderSent()
    {
        LastReminderSentAt = DateTime.UtcNow;
        RemindersSentCount++;

        return Result.Ok();
    }

    internal bool ShouldSendReminder()
    {
        if (IsPaid) return false;
        if (DueDate > DateTime.UtcNow) return false;

        if (LastReminderSentAt.HasValue &&
            LastReminderSentAt.Value.AddDays(1) > DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }
}