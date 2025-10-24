using Domain.Debts.Entities;
using Domain.Debts.Errors;
using Domain.Shared.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Debts.Entities;

public sealed class DebtorTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var name = "João Silva";
        var phoneResult = Phone.Create("11999999999");
        var emailResult = Email.Create("joao@example.com");

        // Act
        var result = Debtor.Create(
            customerId,
            name,
            phoneResult.Value,
            emailResult.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerId.Should().Be(customerId);
        result.Value.Name.Should().Be(name);
        result.Value.Phone.Number.Should().Be("11999999999");
        result.Value.Email!.Address.Should().Be("joao@example.com");
        result.Value.Debts.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithNullEmail_ShouldSucceed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var name = "João Silva";
        var phoneResult = Phone.Create("11999999999");

        // Act
        var result = Debtor.Create(
            customerId,
            name,
            phoneResult.Value,
            null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldReturnError(string? invalidName)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var phoneResult = Phone.Create("11999999999");

        // Act
        var result = Debtor.Create(
            customerId,
            invalidName!,
            phoneResult.Value,
            null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DebtorErrors.NameRequired);
    }

    [Fact]
    public void AddDebt_WithValidData_ShouldAddDebtToCollection()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var phoneResult = Phone.Create("11999999999");
        var debtorResult = Debtor.Create(customerId, "João Silva", phoneResult.Value, null);
        var debtor = debtorResult.Value;

        var amount = 150.00m;
        var dueDate = DateTime.UtcNow.AddDays(30);
        var description = "Mensalidade";

        // Act
        var result = debtor.AddDebt(amount, dueDate, description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        debtor.Debts.Should().HaveCount(1);
        debtor.Debts.First().Amount.Should().Be(amount);
        debtor.Debts.First().DueDate.Should().Be(dueDate);
        debtor.Debts.First().Description.Should().Be(description);
    }

    [Fact]
    public void AddDebt_WithInvalidAmount_ShouldReturnError()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var phoneResult = Phone.Create("11999999999");
        var debtorResult = Debtor.Create(customerId, "João Silva", phoneResult.Value, null);
        var debtor = debtorResult.Value;

        var invalidAmount = -50.00m;
        var dueDate = DateTime.UtcNow.AddDays(30);

        // Act
        var result = debtor.AddDebt(invalidAmount, dueDate, null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DebtErrors.AmountInvalid);
        debtor.Debts.Should().BeEmpty();
    }
}