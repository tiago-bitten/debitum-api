using Domain.Debts.Entities;
using Domain.Debts.Errors;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Debts.Entities;

public sealed class ReminderSentTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var channel = "email";
        var message = "Lembrete de pagamento";

        // Act
        var result = ReminderSent.Create(channel, message);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Channel.Should().Be(channel);
        result.Value.Message.Should().Be(message);
        result.Value.SentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithNullMessage_ShouldSucceed()
    {
        // Arrange
        var debtId = Guid.NewGuid();
        var channel = "sms";

        // Act
        var result = ReminderSent.Create(channel, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Message.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyDebtId_ShouldReturnError()
    {
        // Arrange
        var debtId = Guid.Empty;
        var channel = "email";

        // Act
        var result = ReminderSent.Create(channel);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReminderSentErrors.DebtIdRequired);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidChannel_ShouldReturnError(string? invalidChannel)
    {
        // Arrange
        var debtId = Guid.NewGuid();

        // Act
        var result = ReminderSent.Create(invalidChannel!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReminderSentErrors.ChannelRequired);
    }

    [Theory]
    [InlineData("email")]
    [InlineData("sms")]
    [InlineData("whatsapp")]
    [InlineData("push")]
    public void Create_WithDifferentChannels_ShouldSucceed(string channel)
    {
        // Arrange
        var debtId = Guid.NewGuid();

        // Act
        var result = ReminderSent.Create(channel);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Channel.Should().Be(channel);
    }
}
