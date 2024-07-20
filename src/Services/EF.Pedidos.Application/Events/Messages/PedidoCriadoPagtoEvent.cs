using EF.Core.Commons.Messages.Integrations;

namespace EF.Pedidos.Application.Events.Messages;

public class PedidoCriadoPagtoEvent : IntegrationEvent
{
    public string TipoPagamento { get; set; }
    public decimal ValorTotal { get; set; }
}