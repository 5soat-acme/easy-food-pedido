using EF.Pedidos.Application.DTOs.Responses;

namespace EF.Pedidos.Application.UseCases.Interfaces;

public interface IConsultarPedidoUseCase
{
    Task<PedidoDto?> ObterPedidoPorId(Guid pedidoId);
}