using EF.Core.Commons.Communication;
using EF.Estoques.Application.DTOs.Requests;

namespace EF.Estoques.Application.UseCases.Interfaces;

public interface IAtualizarEstoqueUseCase
{
    Task<OperationResult<Guid>> Handle(AtualizarEstoqueDto dto);
}