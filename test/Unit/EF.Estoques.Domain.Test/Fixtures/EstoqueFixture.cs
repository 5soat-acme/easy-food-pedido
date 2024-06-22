using Bogus;
using EF.Estoques.Domain.Models;
using EF.Test.Utils;

namespace EF.Estoques.Domain.Test.Fixtures;

[CollectionDefinition(nameof(EstoqueCollection))]
public class EstoqueCollection : ICollectionFixture<EstoqueFixture>
{
}

public class EstoqueFixture
{
    public Estoque GerarEstoque(int? qtdEstoque = null)
    {
        return GerarEstoques(1, qtdEstoque).FirstOrDefault()!;
    }

    public Estoque GerarEstoqueInvalido()
    {
        return new Estoque(Guid.Empty, 1);
    }

    public List<Estoque> GerarEstoques(int quantidade, int? qtdEstoque)
    {
        var estoques = new Faker<Estoque>("pt_BR")
            .CustomInstantiator(f => new Estoque(Guid.NewGuid(), qtdEstoque ?? f.Random.Number(1, 10)));

        return estoques.Generate(quantidade);
    }

    public MovimentacaoEstoque GerarMovimentacao(Guid estoqueId, TipoMovimentacaoEstoque? tipoMovimentacao = null,
        OrigemMovimentacaoEstoque? origemMovimentacao = null)
    {
        return GerarMovimentacoes(1, estoqueId, tipoMovimentacao, origemMovimentacao).FirstOrDefault()!;
    }

    public List<MovimentacaoEstoque> GerarMovimentacoes(int quantidade, Guid estoqueId,
        TipoMovimentacaoEstoque? tipoMovimentacaoParam = null,
        OrigemMovimentacaoEstoque? origemMovimentacaoParam = null)
    {
        var tipoMovimentacao = tipoMovimentacaoParam ??
                               UtilsTest.GetRandomEnum<TipoMovimentacaoEstoque>(Enum.GetValues(typeof(TipoMovimentacaoEstoque)));

        OrigemMovimentacaoEstoque origemMovimentacao;
        if (origemMovimentacaoParam is null)
        {
            if (tipoMovimentacao == TipoMovimentacaoEstoque.Entrada)
                origemMovimentacao = OrigemMovimentacaoEstoque.Compra;
            else
                origemMovimentacao = OrigemMovimentacaoEstoque.Venda;
        }
        else
        {
            origemMovimentacao = (OrigemMovimentacaoEstoque)origemMovimentacaoParam;
        }

        var itens = new Faker<MovimentacaoEstoque>("pt_BR")
            .CustomInstantiator(f => new MovimentacaoEstoque(estoqueId, Guid.NewGuid(), f.Random.Number(5),
                tipoMovimentacao, origemMovimentacao!, DateTime.Now));

        return itens.Generate(quantidade);
    }
}