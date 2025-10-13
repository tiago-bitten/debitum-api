using System.Text.RegularExpressions;
using Domain.Shared.Errors;
using Domain.Shared.Results;

namespace Domain.Shared.ValueObjects;

public sealed partial record Email
{
    public string Address { get; } = string.Empty;

    private const string EmailRegexPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    
    private Email(string address) => Address = address;

    public static Result<Email> Create(string address)
    {
        var checkResult = Check(address);
        if (checkResult.IsFailure)
        {
            return checkResult.Error;
        }
        
        return new Email(address);
    }

    public static Result Check(string address)
    {
        if (string.IsNullOrEmpty(address))
            return EmailErrors.AddressNull;
        
        var isValid = EmailRegex().IsMatch(address);
        return isValid ? Result.Ok() : EmailErrors.Invalid;
    }

    [GeneratedRegex(EmailRegexPattern, RegexOptions.IgnoreCase, "pt-BR")]
    private static partial Regex EmailRegex();
    
    public bool Equals(Email? other) => Address.Equals(other?.Address, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => Address.ToLowerInvariant().GetHashCode();
    public override string ToString() => Address;
    
    public static implicit operator string(Email email) => email.Address;
}