namespace Infra.MessageBroker.Consumers;

public sealed class MessageHandlerRegistry : IMessageHandlerRegistry
{
    private readonly Dictionary<string, Type> _handlers = new();

    public void RegisterHandler<TMessage>(string messageType) where TMessage : class
    {
        var handlerType = typeof(IMessageHandler<TMessage>);
        _handlers[messageType] = handlerType;
    }

    public Type? GetHandlerType(string messageType)
    {
        return _handlers.GetValueOrDefault(messageType);
    }
}
