using EF.Core.Commons.Communication;
using EF.Identidade.Application.DTOs.Requests;

namespace EF.Identidade.Application.UseCases.Interfaces;

public interface ICriarSolicitacaoExclusaoUseCase
{
    Task<OperationResult<Guid>> Handle(CriarSolicitacaoExclusaoDto dto);
}
