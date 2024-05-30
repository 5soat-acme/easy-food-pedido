using EF.Pedidos.Application.DTOs.Responses;
using EF.Pedidos.Application.Mappings;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Domain.Repository;

namespace EF.Pedidos.Application.UseCases;

public class ConsultarPedidoUseCase(IPedidoRepository respository) : IConsultarPedidoUseCase
{
    public async Task<PedidoDto?> ObterPedidoPorId(Guid pedidoId)
    {
        var pedido = await respository.ObterPorId(pedidoId);
        return DomainToDtoMapper.Map(pedido);
    }
}