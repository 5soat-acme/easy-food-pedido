using EF.Cupons.Application.DTOs.Responses;
using EF.Cupons.Domain.Models;

namespace EF.Cupons.Application.Mappings;

public static class CupomDomainToDtoMapper
{
    public static CupomDto? Map(Cupom? model)
    {
        if (model is null) return null;

        return new CupomDto
        {
            Id = model.Id,
            DataInicio = model.DataInicio,
            DataFim = model.DataFim,
            PorcentagemDesconto = model.PorcentagemDesconto,
            Produtos = model.CupomProdutos.Select(Map).ToList()
        };
    }

    public static CupomProdutoDto Map(CupomProduto model)
    {
        return new CupomProdutoDto
        {
            ProdutoId = model.ProdutoId
        };
    }
}