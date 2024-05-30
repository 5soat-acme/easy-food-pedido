namespace EF.Core.Commons.Messages;

public abstract class Message
{
    public Message()
    {
        MessageType = GetType().Name;
        Timestamp = DateTime.Now;
    }

    public DateTime Timestamp { get; protected set; }
    public string MessageType { get; protected set; }
    public Guid AggregateId { get; set; }
}