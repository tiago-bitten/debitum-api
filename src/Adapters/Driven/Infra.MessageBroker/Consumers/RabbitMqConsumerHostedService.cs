using Infra.MessageBroker.Configuration;
using Infra.MessageBroker.Connection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infra.MessageBroker.Consumers;

public sealed class RabbitMqConsumerHostedService(
    IRabbitMqConnectionFactory connectionFactory,
    IOptions<RabbitMqOptions> options,
    MessageDispatcher messageDispatcher,
    ILogger<RabbitMqConsumerHostedService> logger)
    : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;
    private IChannel? _channel;
    private IConnection? _connection;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RabbitMQ Consumer Hosted Service is starting");

        try
        {
            _connection = connectionFactory.CreateConnection();
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: _options.PrefetchCount,
                global: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                await ProcessMessageAsync(ea, stoppingToken);
            };

            await _channel.BasicConsumeAsync(
                queue: QueueConfiguration.DebtRemindersQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ Consumer started listening on queue: {Queue}",
                QueueConfiguration.DebtRemindersQueue);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("RabbitMQ Consumer Hosted Service is stopping");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fatal error in RabbitMQ Consumer Hosted Service");
            throw;
        }
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        var messageId = ea.BasicProperties.MessageId ?? "unknown";
        var messageType = ea.BasicProperties.Type ?? string.Empty;

        try
        {
            logger.LogInformation(
                "Processing message - MessageId: {MessageId}, Type: {MessageType}",
                messageId,
                messageType);

            await messageDispatcher.DispatchAsync(messageType, ea.Body, cancellationToken);

            if (_channel != null)
            {
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
            }

            logger.LogInformation(
                "Message processed successfully - MessageId: {MessageId}",
                messageId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error processing message - MessageId: {MessageId}, Type: {MessageType}",
                messageId,
                messageType);

            var retryCount = GetRetryCount(ea.BasicProperties);

            if (_channel != null)
            {
                if (retryCount < _options.MaxRetryAttempts)
                {
                    logger.LogWarning(
                        "Requeuing message - MessageId: {MessageId}, Retry: {RetryCount}/{MaxRetries}",
                        messageId,
                        retryCount + 1,
                        _options.MaxRetryAttempts);

                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: cancellationToken);
                }
                else
                {
                    logger.LogError(
                        "Message exceeded max retries, sending to DLQ - MessageId: {MessageId}",
                        messageId);

                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: cancellationToken);
                }
            }
        }
    }

    private static int GetRetryCount(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers != null && properties.Headers.TryGetValue("x-retry-count", out var value))
        {
            return value is int count ? count : 0;
        }

        return 0;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("RabbitMQ Consumer Hosted Service is stopping");

        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            _channel.Dispose();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            _connection.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
