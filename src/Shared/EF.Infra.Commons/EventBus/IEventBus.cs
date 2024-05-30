using EF.Core.Commons.Messages;

namespace EF.Infra.Commons.EventBus;

public interface IEventBus
{
    void Subscribe<T>(IEventHandler<T> handler) where T : Event;
    Task Publish<T>(T @event) where T : Event;
}