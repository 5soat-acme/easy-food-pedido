using EF.Core.Commons.Communication;
using EF.Cupons.Application.DTOs.Requests;

namespace EF.Cupons.Application.UseCases.Interfaces;

public interface ICriarCupomUseCase
{
    Task<OperationResult<Guid>> Handle(CriarCupomDto dto);
}