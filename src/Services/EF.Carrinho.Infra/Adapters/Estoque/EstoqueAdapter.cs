using EF.Carrinho.Application.Gateways;
using EF.Estoques.Application.UseCases.Interfaces;

namespace EF.Carrinho.Infra.Adapters.Estoque;

public class EstoqueAdapter : IEstoqueService
{
    private readonly IConsultaEstoqueUseCase _consultaEstoqueUseCase;

    public EstoqueAdapter(IConsultaEstoqueUseCase consultaEstoqueUseCase)
    {
        _consultaEstoqueUseCase = consultaEstoqueUseCase;
    }

    public async Task<bool> VerificarEstoque(Guid produtoId, int quantidade)
    {
        return await _consultaEstoqueUseCase.ValidarEstoque(produtoId, quantidade);
    }
}