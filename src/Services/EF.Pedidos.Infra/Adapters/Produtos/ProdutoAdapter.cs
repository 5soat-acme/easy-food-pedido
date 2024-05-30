using EF.Pedidos.Application.DTOs.Gateways;
using EF.Pedidos.Application.Gateways;
using EF.Produtos.Application.UseCases.Interfaces;

namespace EF.Pedidos.Infra.Adapters.Produtos;

public class ProdutoAdapter : IProdutoService
{
    private readonly IConsultarProdutoUseCase _consultarProdutoUseCase;

    public ProdutoAdapter(IConsultarProdutoUseCase consultarProdutoUseCase)
    {
        _consultarProdutoUseCase = consultarProdutoUseCase;
    }

    public async Task<ProdutoDto?> ObterPorId(Guid id)
    {
        var produto = await _consultarProdutoUseCase.BuscarPorId(id);
        return ProdutoToDomainMapper.Map(produto);
    }
}