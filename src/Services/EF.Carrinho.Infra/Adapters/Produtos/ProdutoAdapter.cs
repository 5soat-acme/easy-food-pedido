using EF.Carrinho.Application.Gateways;
using EF.Carrinho.Domain.Models;
using EF.Produtos.Application.UseCases.Interfaces;

namespace EF.Carrinho.Infra.Adapters.Produtos;

public class ProdutoAdapter : IProdutoService
{
    private readonly IConsultarProdutoUseCase _consultarProdutoUseCase;

    public ProdutoAdapter(IConsultarProdutoUseCase consultarProdutoUseCase)
    {
        _consultarProdutoUseCase = consultarProdutoUseCase;
    }

    public async Task<Item> ObterItemPorProdutoId(Guid id)
    {
        var produto = await _consultarProdutoUseCase.BuscarPorId(id);
        return ProdutoToDomainMapper.Map(produto);
    }
}