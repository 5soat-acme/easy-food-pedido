using EF.Core.Commons.Messages;

namespace EF.Infra.Commons.EventBus;

public class InMemoryEventBus : IEventBus
{
    private readonly List<(Type EventType, Delegate Handler)> _handlers = new();

    public void Subscribe<T>(IEventHandler<T> handler) where T : Event
    {
        _handlers.Add((typeof(T), handler.Handle));
    }

    public async Task Publish<T>(T @event) where T : Event
    {
        var eventType = @event.GetType();

        var handlersToInvoke = _handlers
            .Where(x => x.EventType.IsAssignableFrom(eventType))
            .Select(x => x.Handler)
            .ToList();

        foreach (var handler in handlersToInvoke)
            if (handler is Delegate del)
                await (Task)del.DynamicInvoke(@event)!;
    }
}