using Domain.Debts.Errors;
using Domain.Shared.Entities;
using Domain.Shared.Results;

namespace Domain.Debts.Entities;

public sealed class ReminderSent : Entity
{
    private ReminderSent()
    {
    }

    private ReminderSent(string channel, string? message)
        => (Channel, Message) = (channel, message);

    public Guid DebtId { get; private set; }
    public string Channel { get; private set; } = string.Empty;
    public string? Message { get; private set; }
    public DateTimeOffset SentAt { get; private set; } = DateTimeOffset.UtcNow;

    internal static Result<ReminderSent> Create(string channel, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(channel))
            return ReminderSentErrors.ChannelRequired;

        return new ReminderSent(channel, message);
    }
}
