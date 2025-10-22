using Domain.Shared.Events;

namespace Application.Shared.Ports;

public interface IEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
}