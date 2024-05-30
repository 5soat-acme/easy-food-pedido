using EF.Carrinho.Application.DTOs.Requests;
using EF.Core.Commons.Communication;

namespace EF.Carrinho.Application.UseCases.Interfaces;

public interface IRemoverItemCarrinhoUseCase
{
    Task<OperationResult> RemoverItemCarrinho(Guid itemId, CarrinhoSessaoDto carrinhoSessao);
}