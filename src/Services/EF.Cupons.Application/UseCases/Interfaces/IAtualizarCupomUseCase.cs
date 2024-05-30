using EF.Core.Commons.Communication;
using EF.Cupons.Application.DTOs.Requests;

namespace EF.Cupons.Application.UseCases.Interfaces;

public interface IAtualizarCupomUseCase
{
    Task<OperationResult> Handle(AtualizarCupomDto dto);
}