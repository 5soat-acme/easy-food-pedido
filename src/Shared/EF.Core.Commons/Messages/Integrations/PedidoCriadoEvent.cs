namespace EF.Core.Commons.Messages.Integrations;

public class PedidoCriadoEvent : IntegrationEvent
{
    public Guid SessionId { get; set; }
    public Guid? ClienteId { get; set; }
}