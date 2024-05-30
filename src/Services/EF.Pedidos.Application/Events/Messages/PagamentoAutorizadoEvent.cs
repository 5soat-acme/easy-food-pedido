using EF.Core.Commons.Messages.Integrations;

namespace EF.Pedidos.Application.Events.Messages;

public class PagamentoAutorizadoEvent : IntegrationEvent
{
    public Guid PedidoId { get; set; }
}