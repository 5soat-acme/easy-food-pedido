namespace EF.Core.Commons.Messages.Integrations;

public class PedidoCanceladoEvent : IntegrationEvent
{
    public List<ItemPedido> Itens { get; set; }

    public class ItemPedido
    {
        public int Quantidade { get; set; }
        public Guid ProdutoId { get; set; }
    }
}