using Infra.MessageBroker.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infra.MessageBroker.Connection;

public sealed class RabbitMqConnectionFactory(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqConnectionFactory> logger)
    : IRabbitMqConnectionFactory, IDisposable
{
    private readonly RabbitMqOptions _options = options.Value;
    private IConnection? _connection;
    private readonly Lock _lock = new();

    public IConnection CreateConnection()
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        lock (_lock)
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60)
            };

            try
            {
                _connection = factory.CreateConnectionAsync("Debitum-MessageBroker").GetAwaiter().GetResult();
                logger.LogInformation(
                    "RabbitMQ connection established: {Host}:{Port}",
                    _options.Host,
                    _options.Port);

                _connection.ConnectionShutdownAsync += (sender, args) =>
                {
                    logger.LogWarning(
                        "RabbitMQ connection shutdown: {Reason}",
                        args.ReplyText);
                    return Task.CompletedTask;
                };

                return _connection;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create RabbitMQ connection");
                throw;
            }
        }
    }

    public void Dispose()
    {
        try
        {
            _connection?.CloseAsync().GetAwaiter().GetResult();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
    }
}
