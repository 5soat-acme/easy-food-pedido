using EF.Core.Commons.Communication;
using EF.Pedidos.Application.DTOs.Requests;

namespace EF.Pedidos.Application.UseCases.Interfaces;

public interface ICriarPedidoUseCase
{
    Task<OperationResult<Guid>> Handle(CriarPedidoDto criarPedidoDto);
}