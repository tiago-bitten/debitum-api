using Domain.Shared.Events;

namespace Infra.Mongo.Shared.Events;

public interface IEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
}