namespace Infra.MessageBroker.Consumers;

public interface IMessageHandlerRegistry
{
    void RegisterHandler<TMessage>(string messageType) where TMessage : class;
    Type? GetHandlerType(string messageType);
}
