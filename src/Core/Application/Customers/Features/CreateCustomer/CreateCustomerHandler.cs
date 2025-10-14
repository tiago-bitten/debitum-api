using Application.Customers.Ports;
using Application.Shared.Messaging;
using Domain.Customers.Entities;
using Domain.Shared.Results;
using Domain.Shared.ValueObjects;

namespace Application.Customers.Features.CreateCustomer;

internal sealed class CreateCustomerHandler(ICustomerRepository customerRepository)
    : ICommandHandler<CreateCustomerCommand>
{
    public async Task<Result> HandleAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var email = emailResult.Value;

        var phoneResult = Phone.Create(command.Phone);
        if (phoneResult.IsFailure)
            return phoneResult.Error;

        var phone = phoneResult.Value;

        var customerResult = Customer.Create(command.Name, email, phone, command.origem);
        if (customerResult.IsFailure)
            return customerResult.Error;

        var customer = customerResult.Value;

        await customerRepository.AddAsync(customer);

        return Result.Ok();
    }
}