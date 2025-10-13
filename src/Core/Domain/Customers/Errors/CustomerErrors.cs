using Domain.Shared.Results;

namespace Domain.Customers.Errors;

public static class CustomerErrors
{
    public static readonly Error NameRequired =
        Error.Problem("customer.Name", "Name is required.");
}