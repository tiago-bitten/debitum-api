using Application.Shared.Messaging;

namespace Application.Customers.Features.CreateCustomer;

public sealed record CreateCustomerCommand(string Name, string Email, string Phone) : ICommand;