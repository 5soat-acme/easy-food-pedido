using EF.Core.Commons.Messages;

namespace EF.Core.Commons.DomainObjects;

public abstract class Entity
{
    private List<Event> _notifications;

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public IReadOnlyCollection<Event> Notifications => _notifications?.AsReadOnly();

    public void AddEvent(Event @event)
    {
        _notifications = _notifications ?? [];
        _notifications.Add(@event);
    }

    public void RemoveEvent(Event @event)
    {
        _notifications?.Remove(@event);
    }

    public void ClearEvents()
    {
        _notifications?.Clear();
    }

    public override bool Equals(object? obj)
    {
        var compareTo = obj as Entity;

        if (ReferenceEquals(this, compareTo)) return true;
        if (ReferenceEquals(null, compareTo)) return false;

        return Id.Equals(compareTo.Id);
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return GetType().GetHashCode() * 907 + Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }
}