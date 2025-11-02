using Domain.Debts.Errors;
using Domain.Shared.Entities;
using Domain.Shared.Results;

namespace Domain.Debts.Entities;

public sealed class Debt : Entity
{
    private readonly List<ReminderSent> _remindersSent = [];

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

    public IReadOnlyCollection<ReminderSent> RemindersSent => _remindersSent.AsReadOnly();
    public DateTimeOffset? LastReminderSentAt => _remindersSent.MaxBy(r => r.SentAt)?.SentAt;
    public int RemindersSentCount => _remindersSent.Count;

    internal static Result<Debt> Create(decimal amount, DateTime dueDate, string? description = null)
    {
        if (amount <= 0)
            return DebtErrors.AmountInvalid;

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

    public Result RecordReminderSent(string channel, string? message = null)
    {
        var reminderResult = ReminderSent.Create(channel, message);

        if (reminderResult.IsFailure)
            return reminderResult.Error;

        _remindersSent.Add(reminderResult.Value);

        return Result.Ok();
    }

    public bool ShouldSendReminder()
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