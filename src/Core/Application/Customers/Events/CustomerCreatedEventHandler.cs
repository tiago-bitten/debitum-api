using Application.Customers.Ports;
using Domain.Customers.Events;
using Domain.Shared.Events;

namespace Application.Customers.Events;

internal sealed class CustomerCreatedEventHandler(ICustomerRepository customerRepository)
    : IEventHandler<CustomerCreatedEvent>
{
    public async Task HandleAsync(CustomerCreatedEvent @event, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(@event.Id);
        if (customer is null)
        {
            Console.WriteLine($"Customer {@event.Id} not found.");
            ;
        }
    }
}