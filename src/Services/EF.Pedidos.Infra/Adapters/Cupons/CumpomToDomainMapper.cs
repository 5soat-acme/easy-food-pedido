using EF.Pedidos.Application.DTOs.Gateways;
using CupomProdutoDto = EF.Cupons.Application.DTOs.Responses.CupomProdutoDto;

namespace EF.Pedidos.Infra.Adapters.Cupons;

public static class CumpomToDomainMapper
{
    public static CupomDto? Map(EF.Cupons.Application.DTOs.Responses.CupomDto? dto)
    {
        if (dto is null) return null;

        return new CupomDto
        {
            Id = dto.Id,
            PorcentagemDesconto = dto.PorcentagemDesconto,
            Produtos = dto.Produtos.Select(Map).ToList()
        };
    }

    public static CupomDto.CupomProdutoDto Map(CupomProdutoDto dto)
    {
        return new CupomDto.CupomProdutoDto
        {
            ProdutoId = dto.ProdutoId
        };
    }
}