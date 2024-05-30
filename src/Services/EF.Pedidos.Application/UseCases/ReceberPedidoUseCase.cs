using EF.Core.Commons.Communication;
using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.UseCases;
using EF.Pedidos.Application.DTOs.Gateways;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.Events.Messages;
using EF.Pedidos.Application.Gateways;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Domain.Models;
using EF.Pedidos.Domain.Repository;

namespace EF.Pedidos.Application.UseCases;

public class ReceberPedidoUsecase : CommonUseCase, IReceberPedidoUsecase
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoService _produtoService;

    public ReceberPedidoUsecase(IPedidoRepository pedidoRepository, IProdutoService produtoService)
    {
        _pedidoRepository = pedidoRepository;
        _produtoService = produtoService;
    }

    public async Task<OperationResult<Guid>> Handle(ReceberPedidoDto receberPedidoDto)
    {
        var pedido = await _pedidoRepository.ObterPorId(receberPedidoDto.PedidoId);
        if (pedido is null) throw new DomainException("Pedido inválido");

        pedido.ConfirmarPagamento();
        pedido.AddEvent(await CriarEvento(pedido));

        _pedidoRepository.Atualizar(pedido);
        ValidationResult = await PersistData(_pedidoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult<Guid>.Failure(ValidationResult.GetErrorMessages());

        return OperationResult<Guid>.Success(pedido.Id);
    }

    private async Task<List<ProdutoDto>> ObterProdutosPedido(Pedido pedido)
    {
        List<ProdutoDto> produtos = new();
        foreach (var item in pedido.Itens)
        {
            var produto = await _produtoService.ObterPorId(item.ProdutoId);
            produtos.Add(produto);
        }

        return produtos;
    }

    private async Task<PedidoRecebidoEvent> CriarEvento(Pedido pedido)
    {
        var produtos = await ObterProdutosPedido(pedido);
        List<PedidoRecebidoEvent.ItemPedido> itens = new();

        foreach (var item in pedido.Itens)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == item.ProdutoId);
            itens.Add(new PedidoRecebidoEvent.ItemPedido
            {
                Quantidade = item.Quantidade,
                ProdutoId = item.ProdutoId,
                NomeProduto = produto!.Nome,
                TempoPreparoEstimado = produto.TempoPreparoEstimado
            });
        }

        return new PedidoRecebidoEvent
        {
            AggregateId = pedido.Id,
            Itens = itens
        };
    }
}