using EF.Core.Commons.Messages;
using EF.Infra.Commons.Messageria;
using EF.Pedidos.Application.Events.Messages;
using EF.Pedidos.Application.Events.Queues;
using System.Text.Json;

namespace EF.Pedidos.Application.Events;

public class PedidoEventHandler : IEventHandler<PedidoRecebidoEvent>
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
}