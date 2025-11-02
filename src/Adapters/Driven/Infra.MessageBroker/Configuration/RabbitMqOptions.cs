namespace Infra.MessageBroker.Configuration;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";
    public int MaxRetryAttempts { get; init; } = 3;
    public int RetryDelayMilliseconds { get; init; } = 5000;
    public ushort PrefetchCount { get; init; } = 10;
    public int ConsumerMaxConcurrency { get; init; } = 5;
}
