using Bogus;
using EF.Pedidos.Domain.Models;

namespace EF.Pedidos.Domain.Test.Fixtures
{

    [CollectionDefinition(nameof(PedidoCollection))]
    public class PedidoCollection : ICollectionFixture<PedidoFixture>
    {
    }
    public class PedidoFixture
    {
        public Pedido GerarPedido()
        {
            return new Pedido();
        }

        public Item GerarItem(Guid? produtoId = null, string? nome = null, decimal? valorUnitario = null, int? quantidade = null)
        {
            return GerarItens(1, produtoId, nome, valorUnitario, quantidade).FirstOrDefault()!;
        }

        public IList<Item> GerarItens(int qtdGerar, Guid? produtoId = null, string? nome = null, decimal? valorUnitario = null, int? quantidade = null)
        {
            var item = new Faker<Item>("pt_BR")
                .CustomInstantiator(f => new Item(produtoId ?? f.Random.Guid(), nome ?? f.Commerce.ProductName(), valorUnitario ?? f.Random.Decimal(20, 50), quantidade ?? f.Random.Int(1, 20)));

            return item.Generate(qtdGerar);
        }
    }
}
