using EF.Produtos.Application.DTOs.Responses;
using EF.Produtos.Domain.Models;

namespace EF.Produtos.Application.Mappings;

public static class DomainToDtoMapper
{
    public static ProdutoDto? Map(Produto? produto)
    {
        if (produto is null) return null;

        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            ValorUnitario = produto.ValorUnitario,
            TempoPreparoEstimado = produto.TempoPreparoEstimado,
            Categoria = produto.Categoria
        };
    }

    public static IEnumerable<ProdutoDto>? Map(IEnumerable<Produto>? produtos)
    {
        if (produtos is null) return null;

        return produtos.Select(Map).ToList();
    }
}