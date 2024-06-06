using Bogus;
using EF.Cupons.Domain.Models;

namespace EF.Cupons.Domain.Test.Fixtures;

[CollectionDefinition(nameof(CupomCollection))]
public class CupomCollection : ICollectionFixture<CupomFixture>
{
}

public class CupomFixture
{
    public Cupom GerarCupom(DateTime? dataInicio = null, DateTime? dataFim = null, string? codigoCupom = null,
        decimal? porcentagemDesconto = null, CupomStatus? status = null)
    {
        return GerarCupons(1, dataInicio, dataFim, codigoCupom, porcentagemDesconto, status).FirstOrDefault()!;
    }

    public List<Cupom> GerarCupons(int quantidade, DateTime? dataInicio = null, DateTime? dataFim = null,
        string? codigoCupom = null,
        decimal? porcentagemDesconto = null, CupomStatus? status = null)
    {
        var estoques = new Faker<Cupom>("pt_BR")
            .CustomInstantiator(f => new Cupom(dataInicio ?? DateTime.Now,
                dataFim ?? DateTime.Now.AddDays(7),
                codigoCupom ?? $"cupom{f.Random.Number(10000)}",
                porcentagemDesconto ?? ((decimal)f.Random.Number(99) / 100),
                status ?? CupomStatus.Ativo));

        return estoques.Generate(quantidade);
    }

    public CupomProduto GerarCupomProduto(Guid cupomId, Guid? produtoId = null)
    {
        return GerarCupomProdutos(1, cupomId, produtoId).FirstOrDefault()!;
    }

    public List<CupomProduto> GerarCupomProdutos(int quantidade, Guid cupomId, Guid? produtoId = null)
    {
        var itens = new Faker<CupomProduto>("pt_BR")
            .CustomInstantiator(f => new CupomProduto(cupomId, produtoId ?? Guid.NewGuid()));

        return itens.Generate(quantidade);
    }
}