using EF.Estoques.Application.DTOs.Responses;
using EF.Estoques.Domain.Models;

namespace EF.Estoques.Application.Mappings;

public static class EstoqueDomainToDtoMapper
{
    public static EstoqueDto? Map(Estoque? model)
    {
        if (model is null) return null;

        return new EstoqueDto
        {
            ProdutoId = model.ProdutoId,
            Quantidade = model.Quantidade
        };
    }
}