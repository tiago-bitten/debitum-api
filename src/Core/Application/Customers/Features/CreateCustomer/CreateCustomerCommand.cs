using Application.Shared.Messaging;
using Domain.Customers.Enums;

namespace Application.Customers.Features.CreateCustomer;

public sealed record CreateCustomerCommand(string Name, string Email, string Phone, CustomerOrigem origem) : ICommand;