using Domain.Debts.Entities;

namespace Application.Debts.Services;

public interface IReminderMessageBuilder
{
    string BuildMessage(Debtor debtor, Debt debt);
}