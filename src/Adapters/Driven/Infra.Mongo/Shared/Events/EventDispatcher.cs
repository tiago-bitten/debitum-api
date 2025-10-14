using System.Collections.Concurrent;
using Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Mongo.Shared.Events;

internal sealed class EventsDispatcher(IServiceProvider serviceProvider) : IEventsDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeDictionary = new();

    public async Task DispatchAsync(
        IEnumerable<IEvent> events,
        CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            using var scope = serviceProvider.CreateScope();

            var domainEventType = @event.GetType();
            var handlerType = HandlerTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(IEventHandler<>).MakeGenericType(et));

            var handlers = scope.ServiceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler is null)
                    continue;

                var handlerWrapper = HandlerWrapper.Create(handler, domainEventType);

                await handlerWrapper.HandleAsync(@event, cancellationToken);
            }
        }
    }

    private abstract class HandlerWrapper
    {
        public abstract Task HandleAsync(IEvent @event, CancellationToken cancellationToken);

        public static HandlerWrapper Create(object handler, Type domainEventType)
        {
            var wrapperType = WrapperTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(HandlerWrapper<>).MakeGenericType(et));

            return (HandlerWrapper)Activator.CreateInstance(wrapperType, handler)!;
        }
    }

    private sealed class HandlerWrapper<T>(object handler) : HandlerWrapper where T : IEvent
    {
        private readonly IEventHandler<T> _handler = (IEventHandler<T>)handler;

        public override async Task HandleAsync(IEvent @event, CancellationToken cancellationToken)
            => await _handler.HandleAsync((T)@event, cancellationToken);
    }
}