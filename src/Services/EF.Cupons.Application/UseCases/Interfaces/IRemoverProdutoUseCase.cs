using EF.Core.Commons.Communication;
using EF.Cupons.Application.DTOs.Requests;

namespace EF.Cupons.Application.UseCases.Interfaces;

public interface IRemoverProdutoUseCase
{
    Task<OperationResult> Handle(RemoverProdutoDto dto);
}