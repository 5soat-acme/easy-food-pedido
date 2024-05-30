using EF.Core.Commons.Communication;
using EF.Produtos.Application.DTOs.Requests;

namespace EF.Produtos.Application.UseCases.Interfaces;

public interface IAtualizarProdutoUseCase
{
    Task<OperationResult<Guid>> Handle(AtualizarProdutoDto dto);
}