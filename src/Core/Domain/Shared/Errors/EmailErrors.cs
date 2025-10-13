using Domain.Shared.Results;

namespace Domain.Shared.Errors;

public static class EmailErrors
{
    public static readonly Error AddressNull =
        Error.Problem("email.Address", "Email address cannot be null or empty.");
    
    public static readonly Error Invalid =
        Error.Problem("email.Invalid", "Invalid email address.");
}