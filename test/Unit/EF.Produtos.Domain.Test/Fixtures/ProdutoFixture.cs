using Bogus;
using EF.Produtos.Domain.Models;
using EF.Test.Utils;

namespace EF.Produtos.Domain.Test.Fixtures
{
    [CollectionDefinition(nameof(ProdutoCollection))]
    public class ProdutoCollection : ICollectionFixture<ProdutoFixture>
    {
    }
    public class ProdutoFixture
    {
        public Produto GerarProduto(string nome = null, decimal? valorUnitario = null, ProdutoCategoria? categoria = null, 
            int? tempoPreparoEstimado = null, string descricao = null)
        {
            var produto = new Faker<Produto>("pt_BR")
                .CustomInstantiator(f => new Produto(nome ?? f.Commerce.ProductName(), valorUnitario ?? f.Random.Decimal(20, 50),
                categoria ?? UtilsTest.GetRandomEnum<ProdutoCategoria>(Enum.GetValues(typeof(ProdutoCategoria))), 
                tempoPreparoEstimado ?? f.Random.Int(1, 20), descricao ?? f.Commerce.ProductDescription()));

            return produto.Generate();
        }
    }
}
