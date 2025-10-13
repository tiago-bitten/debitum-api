using System.Text.RegularExpressions;
using Domain.Shared.Errors;
using Domain.Shared.Results;

namespace Domain.Shared.ValueObjects;

public sealed partial record Phone 
{
    public string Number { get; } = string.Empty;
    
    private const string PhoneRegexPattern = @"^\+?(\d{2})?(\d{2})(\d{4,5})(\d{4})$";

    private Phone(string number) => Number = number;

    public static Result<Phone> Create(string number)
    {
        var checkResult = Check(number);
        if (checkResult.IsFailure)
        {
            return checkResult.Error;
        }
        
        var cleanedNumber = CleanNumber(number);

        return new Phone(cleanedNumber);
    }

    public static Result Check(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return PhoneErrors.NumberNull;
        
        var cleanedNumber = CleanNumber(number);
        var isValid = PhoneNumberRegex().IsMatch(cleanedNumber);
        
        return isValid ? Result.Ok() : PhoneErrors.Invalid;
    }
    
    private static string CleanNumber(string number) => new(number.Where(char.IsDigit).ToArray());

    [GeneratedRegex(PhoneRegexPattern, RegexOptions.IgnoreCase, "pt-BR")]
    private static partial Regex PhoneNumberRegex();

    public bool Equals(Phone? other) => Number.Equals(other?.Number, StringComparison.Ordinal);
    public override int GetHashCode() => Number.GetHashCode();
    public override string ToString() => Number;
    
    public static implicit operator string(Phone phone) => phone.Number;
}