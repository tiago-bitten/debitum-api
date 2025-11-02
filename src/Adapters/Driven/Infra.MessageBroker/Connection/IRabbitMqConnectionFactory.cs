using RabbitMQ.Client;

namespace Infra.MessageBroker.Connection;

public interface IRabbitMqConnectionFactory
{
    IConnection CreateConnection();
}
