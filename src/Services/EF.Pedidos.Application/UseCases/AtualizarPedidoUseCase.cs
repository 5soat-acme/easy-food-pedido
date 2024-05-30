using EF.Core.Commons.Communication;
using EF.Core.Commons.UseCases;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Domain.Repository;

namespace EF.Pedidos.Application.UseCases;

public class AtualizarPedidoUseCase : CommonUseCase, IAtualizarPedidoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public AtualizarPedidoUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<OperationResult> Handle(AtualizarPedidoDto atualizarPedidoDto)
    {
        var pedido = await _pedidoRepository.ObterPorId(atualizarPedidoDto.PedidoId);
        pedido.AtualizarStatus(atualizarPedidoDto.Status);
        _pedidoRepository.Atualizar(pedido);
        await PersistData(_pedidoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult.Failure(ValidationResult);

        return OperationResult.Success();
    }
}