using EF.Core.Commons.Communication;
using EF.Produtos.Application.DTOs.Requests;

namespace EF.Produtos.Application.UseCases.Interfaces;

public interface ICriarProdutoUseCase
{
    Task<OperationResult<Guid>> Handle(CriarProdutoDto dto);
}