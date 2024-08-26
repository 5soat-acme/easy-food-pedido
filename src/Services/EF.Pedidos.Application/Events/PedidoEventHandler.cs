using EF.Core.Commons.Messages;
using EF.Core.Commons.Messages.Integrations;
using EF.Infra.Commons.Messageria;
using EF.Pedidos.Application.Events.Messages;
using EF.Pedidos.Application.Events.Queues;
using System.Text.Json;

namespace EF.Pedidos.Application.Events;

public class PedidoEventHandler : IEventHandler<PedidoRecebidoEvent>, IEventHandler<PedidoCriadoEvent>
{
    private readonly IProducer _producer;

    public PedidoEventHandler(IProducer producer)
    {
        _producer = producer;
    }

    public async Task Handle(PedidoRecebidoEvent notification)
    {
        await _producer.SendMessageAsync(QueuesNames.PedidoRecebido.ToString(), JsonSerializer.Serialize(notification));
    }

    public async Task Handle(PedidoCriadoEvent notification)
    {
        var pedidoCriadoPagamentoEvent = new PedidoCriadoPagtoEvent()
        {
            AggregateId = notification.AggregateId,
            TipoPagamento = notification.Pagamento.TipoPagamento,
            ValorTotal = notification.Pagamento.ValorTotal
        };

        await _producer.SendMessageAsync(QueuesNames.PedidoCriado.ToString(), JsonSerializer.Serialize(pedidoCriadoPagamentoEvent));
    }
}