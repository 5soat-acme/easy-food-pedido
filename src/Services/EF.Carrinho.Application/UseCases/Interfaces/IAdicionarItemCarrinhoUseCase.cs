using EF.Carrinho.Application.DTOs.Requests;
using EF.Core.Commons.Communication;

namespace EF.Carrinho.Application.UseCases.Interfaces;

public interface IAdicionarItemCarrinhoUseCase
{
    Task<OperationResult> AdicionarItemCarrinho(AdicionarItemDto itemDto, CarrinhoSessaoDto carrinhoSessao);
}