using Application.Shared.Ports;
using Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Postgres.Shared.Events;

internal sealed class EventsDispatcher(IServiceProvider serviceProvider) : IEventsDispatcher
{
    public async Task DispatchAsync(
        IEnumerable<IEvent> events,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        foreach (var @event in events)
        {
            var domainEventType = @event.GetType();
            var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEventType);
            var handlers = scope.ServiceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var handlerWrapper = HandlerWrapper.Create(handler, domainEventType);
                await handlerWrapper.HandleAsync(@event, cancellationToken);
            }
        }
    }

    private abstract class HandlerWrapper
    {
        public abstract Task HandleAsync(IEvent @event, CancellationToken cancellationToken);

        public static HandlerWrapper Create(object handler, Type eventType)
        {
            var wrapperType = typeof(HandlerWrapperImpl<>).MakeGenericType(eventType);
            return (HandlerWrapper)Activator.CreateInstance(wrapperType, handler)!;
        }

        private sealed class HandlerWrapperImpl<TEvent>(IEventHandler<TEvent> handler) : HandlerWrapper
            where TEvent : IEvent
        {
            public override Task HandleAsync(IEvent @event, CancellationToken cancellationToken)
            {
                return handler.HandleAsync((TEvent)@event, cancellationToken);
            }
        }
    }
}
