using Application.Debts.Ports;
using Application.Shared.Messaging;
using Domain.Debts.Entities;
using Domain.Shared.Results;
using Domain.Shared.ValueObjects;

namespace Application.Debts.Features.RegisterDebtor;

internal sealed class RegisterDebtorHandler(IDebtorRepository debtorRepository)
    : ICommandHandler<RegisterDebtorCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(
        RegisterDebtorCommand command,
        CancellationToken cancellationToken = default)
    {
        var phoneResult = Phone.Create(command.Phone);
        if (phoneResult.IsFailure)
            return phoneResult.Error;

        var phone = phoneResult.Value;

        Email? email = null;
        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            var emailResult = Email.Create(command.Email);
            if (emailResult.IsFailure)
                return emailResult.Error;

            email = emailResult.Value;
        }

        var debtorResult = Debtor.Create(command.CustomerId, command.Name, phone, email);
        if (debtorResult.IsFailure)
            return debtorResult.Error;

        var debtor = debtorResult.Value;

        await debtorRepository.AddAsync(debtor);

        return debtor.Id;
    }
}