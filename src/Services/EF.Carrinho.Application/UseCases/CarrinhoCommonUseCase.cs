using EF.Carrinho.Application.Gateways;
using EF.Carrinho.Domain.Models;
using EF.Core.Commons.UseCases;

namespace EF.Carrinho.Application.UseCases;

public abstract class CarrinhoCommonUseCase : CommonUseCase
{
    private readonly IEstoqueService _estoqueService;

    protected CarrinhoCommonUseCase(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    protected async Task<bool> ValidarEstoque(Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        return await _estoqueService.VerificarEstoque(item.ProdutoId, item.Quantidade);
    }
}