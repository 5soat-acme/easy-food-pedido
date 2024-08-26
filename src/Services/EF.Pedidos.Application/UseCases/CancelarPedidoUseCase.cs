using EF.Core.Commons.Communication;
using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.Messages.Integrations;
using EF.Core.Commons.UseCases;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Domain.Repository;

namespace EF.Pedidos.Application.UseCases;

public class CancelarPedidoUseCase : CommonUseCase, ICancelarPedidoUseCase
{
    private readonly IPedidoRepository _pedidoRepository;

    public CancelarPedidoUseCase(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<OperationResult<Guid>> Handle(CancelarPedidoDto cancelarPedidoDto)
    {
        var pedido = await _pedidoRepository.ObterPorId(cancelarPedidoDto.PedidoId);
        if (pedido is null) throw new DomainException("Pedido inválido");

        pedido.CancelarPedido();

        _pedidoRepository.Atualizar(pedido);
        pedido.AddEvent(new PedidoCanceladoEvent
        {
            AggregateId = pedido.Id,
            Itens = pedido.Itens.Select(x => new PedidoCanceladoEvent.ItemPedido()
            {
                ProdutoId = x.ProdutoId,
                Quantidade = x.Quantidade
            }).ToList(),
        });

        ValidationResult = await PersistData(_pedidoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult<Guid>.Failure(ValidationResult.GetErrorMessages());

        return OperationResult<Guid>.Success(pedido.Id);
    }
}