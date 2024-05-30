using EF.Core.Commons.Communication;

namespace EF.Produtos.Application.UseCases.Interfaces;

public interface IRemoverProdutoUseCase
{
    Task<OperationResult> Handle(Guid id);
}