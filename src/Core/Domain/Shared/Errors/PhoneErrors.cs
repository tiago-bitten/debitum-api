using Domain.Shared.Results;

namespace Domain.Shared.Errors;

public static class PhoneErrors
{
    public static readonly Error NumberNull = 
        Error.Problem("phone.numberNull", "Phone number is null.");
    
    public static readonly Error Invalid =
        Error.Problem("phone.invalidLength", "Phone is invalid.");
}