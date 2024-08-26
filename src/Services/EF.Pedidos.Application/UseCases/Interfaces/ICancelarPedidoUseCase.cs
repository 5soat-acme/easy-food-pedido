using EF.Core.Commons.Communication;
using EF.Pedidos.Application.DTOs.Requests;

namespace EF.Pedidos.Application.UseCases.Interfaces;

public interface ICancelarPedidoUseCase
{
    Task<OperationResult<Guid>> Handle(CancelarPedidoDto cancelarPedidoDto);
}