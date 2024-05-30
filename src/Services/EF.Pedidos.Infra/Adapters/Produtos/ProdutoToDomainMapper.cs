using EF.Pedidos.Application.DTOs.Gateways;

namespace EF.Pedidos.Infra.Adapters.Produtos;

public static class ProdutoToDomainMapper
{
    public static ProdutoDto? Map(EF.Produtos.Application.DTOs.Responses.ProdutoDto? dto)
    {
        if (dto is null) return null;

        return new ProdutoDto
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            ValorUnitario = dto.ValorUnitario,
            TempoPreparoEstimado = dto.TempoPreparoEstimado
        };
    }
}