using Bogus;
using EF.Carrinho.Domain.Models;

namespace EF.Carrinho.Domain.Test.Fixtures;

[CollectionDefinition(nameof(CarrinhoCollection))]
public class CarrinhoCollection : ICollectionFixture<CarrinhoClienteFixture>
{
}

public class CarrinhoClienteFixture
{

    public CarrinhoCliente GerarCarrinho()
    {
        return new CarrinhoCliente();
    }

    public Item GerarItem(Guid? produtoId = null, string? nome = null, decimal? valorUnitario = null, int? tempoEstimadoPreparo = null)
    {
        return GerarItens(1, produtoId, nome, valorUnitario, tempoEstimadoPreparo).FirstOrDefault()!;
    }

    public IList<Item> GerarItens(int qtdGerar, Guid? produtoId = null, string? nome = null, decimal? valorUnitario = null, int? tempoEstimadoPreparo = null)
    {
        var item = new Faker<Item>("pt_BR")
            .CustomInstantiator(f => new Item(produtoId ?? f.Random.Guid(), nome ?? f.Commerce.ProductName(), valorUnitario ?? f.Random.Decimal(20, 50), tempoEstimadoPreparo ?? f.Random.Int(1, 20)));

        return item.Generate(qtdGerar);
    }
}