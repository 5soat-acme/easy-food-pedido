using EF.Core.Commons.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace EF.Infra.Commons.EventBus;

public static class EventBusExtension
{
    public static async Task PublishEvents<T>(this IEventBus eventBus, T ctx) where T : DbContext
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.Notifications != null && x.Entity.Notifications.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Notifications)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearEvents());

        var tasks = domainEvents
            .Select(async domainEvent => { await eventBus.Publish(domainEvent); });

        await Task.WhenAll(tasks);
    }
}