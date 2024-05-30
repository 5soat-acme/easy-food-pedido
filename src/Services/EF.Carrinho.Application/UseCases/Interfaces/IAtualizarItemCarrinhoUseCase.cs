using EF.Carrinho.Application.DTOs.Requests;
using EF.Core.Commons.Communication;

namespace EF.Carrinho.Application.UseCases.Interfaces;

public interface IAtualizarItemCarrinhoUseCase
{
    Task<OperationResult> AtualizarItem(AtualizarItemDto itemDto, CarrinhoSessaoDto carrinhoSessao);
}