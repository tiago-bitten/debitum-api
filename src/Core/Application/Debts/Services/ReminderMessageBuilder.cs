using Domain.Debts.Entities;

namespace Application.Debts.Services;

internal sealed class ReminderMessageBuilder : IReminderMessageBuilder
{
    public string BuildMessage(Debtor debtor, Debt debt)
    {
        return $"Olá {debtor.Name}! 👋\n\n" +
               $"Este é um lembrete sobre o pagamento pendente de *R$ {debt.Amount:F2}*.\n" +
               $"📅 Data de vencimento: {debt.DueDate:dd/MM/yyyy}\n\n" +
               (string.IsNullOrWhiteSpace(debt.Description)
                   ? ""
                   : $"📝 Descrição: {debt.Description}\n\n") +
               "Por favor, regularize sua situação o quanto antes. " +
               "Caso já tenha efetuado o pagamento, desconsidere esta mensagem.";
    }
}