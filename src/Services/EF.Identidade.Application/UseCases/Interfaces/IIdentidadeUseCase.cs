using EF.Core.Commons.Communication;
using EF.Identidade.Application.DTOs.Responses;

namespace EF.Identidade.Application.UseCases.Interfaces;

public interface IIdentidadeUseCase
{
    Task<OperationResult<RespostaTokenAcesso>> AcessarSistema();
}