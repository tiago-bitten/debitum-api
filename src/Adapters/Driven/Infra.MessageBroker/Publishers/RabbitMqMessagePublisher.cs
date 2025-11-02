using System.Text;
using System.Text.Json;
using Application.Shared.Ports;
using Infra.MessageBroker.Configuration;
using Infra.MessageBroker.Connection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;

namespace Infra.MessageBroker.Publishers;

public sealed class RabbitMqMessagePublisher : IMessagePublisher
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqMessagePublisher> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public RabbitMqMessagePublisher(
        IRabbitMqConnectionFactory connectionFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqMessagePublisher> logger)
    {
        _connectionFactory = connectionFactory;
        var options1 = options.Value;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: options1.MaxRetryAttempts,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromMilliseconds(options1.RetryDelayMilliseconds * retryAttempt),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount} of {MaxRetries} after {Delay}ms",
                        retryCount,
                        options1.MaxRetryAttempts,
                        timeSpan.TotalMilliseconds);
                });
    }

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            var messageType = typeof(TMessage).Name;
            var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true,
                DeliveryMode = DeliveryModes.Persistent,
                ContentType = "application/json",
                Type = messageType,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                MessageId = Guid.NewGuid().ToString()
            };

            await channel.BasicPublishAsync(
                exchange: QueueConfiguration.DebtRemindersExchange,
                routingKey: QueueConfiguration.DebtRemindersRoutingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Message published to RabbitMQ - Type: {MessageType}, MessageId: {MessageId}, Exchange: {Exchange}",
                messageType,
                properties.MessageId,
                QueueConfiguration.DebtRemindersExchange);
        });
    }
}
