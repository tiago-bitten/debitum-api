using Infra.MessageBroker.Configuration;
using Infra.MessageBroker.Connection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infra.MessageBroker.Setup;

public sealed class RabbitMqSetup(
    IRabbitMqConnectionFactory connectionFactory,
    ILogger<RabbitMqSetup> logger)
{
    public void Initialize()
    {
        logger.LogInformation("Initializing RabbitMQ topology...");

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

        channel.ExchangeDeclareAsync(
            exchange: QueueConfiguration.DebtRemindersExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false).GetAwaiter().GetResult();

        logger.LogInformation(
            "Exchange declared: {Exchange}",
            QueueConfiguration.DebtRemindersExchange);

        channel.QueueDeclareAsync(
            queue: QueueConfiguration.DebtRemindersDeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null).GetAwaiter().GetResult();

        logger.LogInformation(
            "Dead letter queue declared: {Queue}",
            QueueConfiguration.DebtRemindersDeadLetterQueue);

        var queueArguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", QueueConfiguration.DebtRemindersDeadLetterQueue },
            { "x-queue-type", "quorum" }
        };

        channel.QueueDeclareAsync(
            queue: QueueConfiguration.DebtRemindersQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArguments).GetAwaiter().GetResult();

        logger.LogInformation(
            "Main queue declared: {Queue}",
            QueueConfiguration.DebtRemindersQueue);

        channel.QueueBindAsync(
            queue: QueueConfiguration.DebtRemindersQueue,
            exchange: QueueConfiguration.DebtRemindersExchange,
            routingKey: QueueConfiguration.DebtRemindersRoutingKey).GetAwaiter().GetResult();

        logger.LogInformation(
            "Queue bound to exchange: {Queue} -> {Exchange} (routing key: {RoutingKey})",
            QueueConfiguration.DebtRemindersQueue,
            QueueConfiguration.DebtRemindersExchange,
            QueueConfiguration.DebtRemindersRoutingKey);

        logger.LogInformation("RabbitMQ topology initialized successfully");
    }
}
