using EF.Core.Commons.Communication;
using EF.Core.Commons.DomainObjects;
using EF.Core.Commons.Messages.Integrations;
using EF.Core.Commons.UseCases;
using EF.Core.Commons.ValueObjects;
using EF.Pedidos.Application.DTOs.Gateways;
using EF.Pedidos.Application.DTOs.Requests;
using EF.Pedidos.Application.Gateways;
using EF.Pedidos.Application.UseCases.Interfaces;
using EF.Pedidos.Domain.Models;
using EF.Pedidos.Domain.Repository;

namespace EF.Pedidos.Application.UseCases;

public class CriarPedidoUseCase : CommonUseCase, ICriarPedidoUseCase
{
    private readonly ICupomService _cupomService;
    private readonly IEstoqueService _estoqueService;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoService _produtoService;

    public CriarPedidoUseCase(IPedidoRepository pedidoRepository,
        IEstoqueService estoqueService, ICupomService cupomService, IProdutoService produtoService)
    {
        _pedidoRepository = pedidoRepository;
        _estoqueService = estoqueService;
        _cupomService = cupomService;
        _produtoService = produtoService;
    }

    public async Task<OperationResult<Guid>> Handle(CriarPedidoDto criarPedidoDto)
    {
        var pedido = await MapearPedido(criarPedidoDto);

        if (!string.IsNullOrEmpty(criarPedidoDto.CodigoCupom))
            pedido = await AplicarCupom(criarPedidoDto.CodigoCupom, pedido);

        pedido.CalcularValorTotal();

        if (!await ValidarPedido(pedido)) return OperationResult<Guid>.Failure(ValidationResult);

        _pedidoRepository.Criar(pedido);
        pedido.AddEvent(new PedidoCriadoEvent
        {
            AggregateId = pedido.Id,
            SessionId = criarPedidoDto.SessionId,
            ClienteId = criarPedidoDto.ClienteId
        });
        await PersistData(_pedidoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult<Guid>.Failure(ValidationResult);

        return OperationResult<Guid>.Success(pedido.Id);
    }

    private async Task<Pedido> MapearPedido(CriarPedidoDto criarPedidoDto)
    {
        var produtos = await ObterProdutosPedido(criarPedidoDto);
        var pedido = new Pedido();

        if (criarPedidoDto.ClienteId.HasValue) pedido.AssociarCliente(criarPedidoDto.ClienteId.Value);

        if (!string.IsNullOrEmpty(criarPedidoDto.ClienteCpf))
        {
            var cpf = new Cpf(criarPedidoDto.ClienteCpf);
            pedido.AssociarCpf(cpf);
        }

        foreach (var itemAdd in criarPedidoDto.Itens)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == itemAdd.ProdutoId);

            var item = new Item(produto!.Id, produto.Nome, produto.ValorUnitario, itemAdd.Quantidade);
            pedido.AdicionarItem(item);
        }

        return pedido;
    }

    private async Task<Pedido> AplicarCupom(string codigoCupom, Pedido pedido)
    {
        var cupom = await _cupomService.ObterCupomPorCodigo(codigoCupom);

        if (cupom is null) return pedido;

        pedido.AssociarCupom(cupom.Id);

        foreach (var item in pedido.Itens)
            if (cupom.Produtos.Exists(produto => produto.ProdutoId == item.ProdutoId))
                pedido.AplicarDescontoItem(item.Id, cupom.PorcentagemDesconto);

        return pedido;
    }

    private async Task<bool> ValidarPedido(Pedido pedido)
    {
        foreach (var item in pedido.Itens)
            if (!await ValidarEstoque(item))
                AddError("Estoque insuficiente", item.Id.ToString());

        return ValidationResult.IsValid;
    }

    private async Task<bool> ValidarEstoque(Item item)
    {
        return await _estoqueService.VerificarEstoque(item.ProdutoId, item.Quantidade);
    }

    private async Task<List<ProdutoDto>> ObterProdutosPedido(CriarPedidoDto criarPedidoDto)
    {
        List<ProdutoDto> produtos = new();
        foreach (var item in criarPedidoDto.Itens)
        {
            var produto = await _produtoService.ObterPorId(item.ProdutoId);
            if (produto is null) throw new DomainException("Produto inválido");
            produtos.Add(produto);
        }

        return produtos;
    }
}