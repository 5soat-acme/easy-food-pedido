using EF.Estoques.Application.DTOs.Responses;

namespace EF.Estoques.Application.UseCases.Interfaces;

public interface IConsultaEstoqueUseCase
{
    Task<EstoqueDto?> ObterEstoqueProduto(Guid produtoId, CancellationToken cancellationToken);

    Task<bool> ValidarEstoque(Guid produtoId, int quantidade,
        CancellationToken cancellationToken = default);
}