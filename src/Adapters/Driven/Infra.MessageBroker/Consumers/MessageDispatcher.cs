using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infra.MessageBroker.Consumers;

public sealed class MessageDispatcher(
    IServiceProvider serviceProvider,
    IMessageHandlerRegistry handlerRegistry,
    ILogger<MessageDispatcher> logger)
{
    public async Task DispatchAsync(string messageType, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default)
    {
        var handlerType = handlerRegistry.GetHandlerType(messageType);

        if (handlerType == null)
        {
            logger.LogWarning("No handler registered for message type: {MessageType}", messageType);
            return;
        }

        var json = Encoding.UTF8.GetString(body.ToArray());

        var messageInterfaceType = handlerType.GetGenericArguments()[0];

        var message = JsonSerializer.Deserialize(json, messageInterfaceType, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (message == null)
        {
            logger.LogWarning("Failed to deserialize message of type: {MessageType}", messageType);
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        var handleMethod = handlerType.GetMethod("HandleAsync");
        if (handleMethod != null)
        {
            var task = (Task)handleMethod.Invoke(handler, [message, cancellationToken])!;
            await task;
        }
    }
}
