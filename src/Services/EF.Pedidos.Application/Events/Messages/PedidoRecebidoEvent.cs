using EF.Core.Commons.Messages.Integrations;

namespace EF.Pedidos.Application.Events.Messages;

public class PedidoRecebidoEvent : IntegrationEvent
{
    public List<ItemPedido> Itens { get; set; }

    public class ItemPedido
    {
        public int Quantidade { get; set; }
        public Guid ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int TempoPreparoEstimado { get; set; }
    }
}