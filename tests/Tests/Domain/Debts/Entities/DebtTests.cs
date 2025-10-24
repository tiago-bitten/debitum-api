using Domain.Debts.Entities;
using Domain.Debts.Errors;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Debts.Entities;

public sealed class DebtTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var amount = 150.00m;
        var dueDate = DateTime.UtcNow.AddDays(30).Date;
        var description = "Mensalidade de novembro";

        // Act
        var result = Debt.Create(amount, dueDate, description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(amount);
        result.Value.DueDate.Should().Be(dueDate);
        result.Value.Description.Should().Be(description);
        result.Value.IsPaid.Should().BeFalse();
        result.Value.PaidAt.Should().BeNull();
        result.Value.RemindersSentCount.Should().Be(0);
        result.Value.LastReminderSentAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullDescription_ShouldSucceed()
    {
        // Arrange
        var amount = 150.00m;
        var dueDate = DateTime.UtcNow.AddDays(30).Date;

        // Act
        var result = Debt.Create(amount, dueDate, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-100.50)]
    public void Create_WithInvalidAmount_ShouldReturnError(decimal invalidAmount)
    {
        // Arrange
        var dueDate = DateTime.UtcNow.AddDays(30);

        // Act
        var result = Debt.Create(invalidAmount, dueDate, null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DebtErrors.AmountInvalid);
    }

    [Fact]
    public void MarkAsPaid_WhenNotPaid_ShouldMarkAsPaid()
    {
        // Arrange
        var debtResult = Debt.Create(150.00m, DateTime.UtcNow.AddDays(30), null);
        var debt = debtResult.Value;

        // Act
        var result = debt.MarkAsPaid();

        // Assert
        result.IsSuccess.Should().BeTrue();
        debt.IsPaid.Should().BeTrue();
        debt.PaidAt.Should().NotBeNull();
        debt.PaidAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkAsPaid_WhenAlreadyPaid_ShouldReturnError()
    {
        // Arrange
        var debtResult = Debt.Create(150.00m, DateTime.UtcNow.AddDays(30), null);
        var debt = debtResult.Value;
        debt.MarkAsPaid();

        // Act
        var result = debt.MarkAsPaid();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DebtErrors.AlreadyPaid);
    }

    [Fact]
    public void RecordReminderSent_ShouldUpdateCountersAndTimestamp()
    {
        // Arrange
        var debtResult = Debt.Create(150.00m, DateTime.UtcNow.AddDays(30), null);
        var debt = debtResult.Value;

        // Act
        var result = debt.RecordReminderSent();

        // Assert
        result.IsSuccess.Should().BeTrue();
        debt.RemindersSentCount.Should().Be(1);
        debt.LastReminderSentAt.Should().NotBeNull();
        debt.LastReminderSentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void RecordReminderSent_CalledMultipleTimes_ShouldIncrementCounter()
    {
        // Arrange
        var debtResult = Debt.Create(150.00m, DateTime.UtcNow.AddDays(30), null);
        var debt = debtResult.Value;

        // Act
        debt.RecordReminderSent();
        debt.RecordReminderSent();
        debt.RecordReminderSent();

        // Assert
        debt.RemindersSentCount.Should().Be(3);
    }

    [Fact]
    public void ShouldSendReminder_WhenDebtIsPaid_ShouldReturnFalse()
    {
        // Arrange
        var debtResult = Debt.Create(150.00m, DateTime.UtcNow.AddDays(-10), null);
        var debt = debtResult.Value;
        debt.MarkAsPaid();

        // Act
        var shouldSend = debt.ShouldSendReminder();

        // Assert
        shouldSend.Should().BeFalse();
    }

    [Fact]
    public void ShouldSendReminder_WhenDueDateIsInFuture_ShouldReturnFalse()
    {
        // Arrange
        var debtResult = Debt.Create(150.00m, DateTime.UtcNow.AddDays(30), null);
        var debt = debtResult.Value;

        // Act
        var shouldSend = debt.ShouldSendReminder();

        // Assert
        shouldSend.Should().BeFalse();
    }

    [Fact]
    public void ShouldSendReminder_WhenReminderSentToday_ShouldReturnFalse()
    {
        // Arrange
        var debtResult = Debt.Create(150.00m, DateTime.UtcNow.AddDays(-10), null);
        var debt = debtResult.Value;
        debt.RecordReminderSent();

        // Act
        var shouldSend = debt.ShouldSendReminder();

        // Assert
        shouldSend.Should().BeFalse();
    }

    [Fact]
    public void ShouldSendReminder_WhenOverdueAndNoRecentReminder_ShouldReturnTrue()
    {
        // Arrange
        var debtResult = Debt.Create(150.00m, DateTime.UtcNow.AddDays(-10), null);
        var debt = debtResult.Value;

        // Act
        var shouldSend = debt.ShouldSendReminder();

        // Assert
        shouldSend.Should().BeTrue();
    }
}