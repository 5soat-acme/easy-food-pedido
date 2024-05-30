using EF.Carrinho.Domain.Models;
using EF.Produtos.Application.DTOs.Responses;

namespace EF.Carrinho.Infra.Adapters.Produtos;

public static class ProdutoToDomainMapper
{
    public static Item? Map(ProdutoDto? model)
    {
        if (model is null) return null;

        return new Item(model.Id, model.Nome, model.ValorUnitario, model.TempoPreparoEstimado);
    }
}