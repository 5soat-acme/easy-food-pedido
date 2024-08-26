namespace EF.Core.Commons.Messages.Integrations;

public class PedidoCriadoEvent : IntegrationEvent
{
    public Guid SessionId { get; set; }
    public Guid? ClienteId { get; set; }

    public List<ItemPedido> Itens { get; set; }
    public PagamentoInfo Pagamento { get; set; }

    public class ItemPedido
    {
        public int Quantidade { get; set; }
        public Guid ProdutoId { get; set; }
    }

    public class PagamentoInfo
    {
        public string TipoPagamento { get; set; }
        public decimal ValorTotal { get; set; }
    }
}