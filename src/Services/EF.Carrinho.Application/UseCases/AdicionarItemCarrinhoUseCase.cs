using EF.Carrinho.Application.DTOs.Requests;
using EF.Carrinho.Application.Gateways;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Domain.Models;
using EF.Carrinho.Domain.Repository;
using EF.Core.Commons.Communication;
using EF.Core.Commons.DomainObjects;

namespace EF.Carrinho.Application.UseCases;

public class AdicionarItemCarrinhoUseCase : CarrinhoCommonUseCase, IAdicionarItemCarrinhoUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IConsultarCarrinhoUseCase _consultarCarrinhoUseCase;
    private readonly IProdutoService _produtoService;

    public AdicionarItemCarrinhoUseCase(
        ICarrinhoRepository carrinhoRepository, IEstoqueService estoqueService,
        IProdutoService produtoService, IConsultarCarrinhoUseCase consultarCarrinhoUseCase) : base(estoqueService)
    {
        _carrinhoRepository = carrinhoRepository;
        _produtoService = produtoService;
        _consultarCarrinhoUseCase = consultarCarrinhoUseCase;
    }

    public async Task<OperationResult> AdicionarItemCarrinho(AdicionarItemDto itemDto, CarrinhoSessaoDto carrinhoSessao)
    {
        var itemAdicionar = await _produtoService.ObterItemPorProdutoId(itemDto.ProdutoId);
        if (itemAdicionar is null) throw new DomainException("Produto inválido");

        var carrinho = await _consultarCarrinhoUseCase.ObterCarrinho(carrinhoSessao);

        if (carrinho is null)
        {
            carrinho = await AdicionarItemCarrinhoNovo(itemDto, carrinhoSessao);
            _carrinhoRepository.Criar(carrinho);
        }
        else
        {
            carrinho = await AdicionarItemCarrinhoExistente(carrinho, itemDto);
            _carrinhoRepository.Atualizar(carrinho);
        }

        var item = carrinho.ObterItemPorProdutoId(itemDto.ProdutoId);

        if (!await ValidarEstoque(item!)) return OperationResult.Failure("Produto sem estoque");

        await PersistData(_carrinhoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult.Failure(ValidationResult);

        return OperationResult.Success();
    }

    private async Task<CarrinhoCliente> AdicionarItemCarrinhoNovo(AdicionarItemDto itemDto,
        CarrinhoSessaoDto carrinhoSessao)
    {
        var item = await _produtoService.ObterItemPorProdutoId(itemDto.ProdutoId);
        item.AtualizarQuantidade(itemDto.Quantidade);
        var carrinho = CriarCarrinhoCliente(carrinhoSessao);
        carrinho.AdicionarItem(item);
        return carrinho;
    }

    private async Task<CarrinhoCliente> AdicionarItemCarrinhoExistente(CarrinhoCliente carrinho,
        AdicionarItemDto itemDto)
    {
        var produtoExiste = carrinho.ProdutoExiste(itemDto.ProdutoId);

        if (produtoExiste)
        {
            var itemExistente = carrinho.ObterItemPorProdutoId(itemDto.ProdutoId);
            var quantidade = itemExistente.Quantidade + itemDto.Quantidade;
            carrinho.AtualizarQuantidadeItem(itemExistente.Id, quantidade);
            var itemAtualizado = carrinho.ObterItemPorProdutoId(itemDto.ProdutoId);
            _carrinhoRepository.AtualizarItem(itemAtualizado);
        }
        else
        {
            var itemNovo = await _produtoService.ObterItemPorProdutoId(itemDto.ProdutoId);
            carrinho.AdicionarItem(itemNovo);
            carrinho.AtualizarQuantidadeItem(itemNovo.Id, itemDto.Quantidade);
            _carrinhoRepository.AdicionarItem(itemNovo);
        }

        return carrinho;
    }

    private CarrinhoCliente CriarCarrinhoCliente(CarrinhoSessaoDto carrinhoSessao)
    {
        var carrinho = new CarrinhoCliente();

        if (carrinhoSessao.ClienteId.HasValue) carrinho.AssociarCliente(carrinhoSessao.ClienteId.Value);

        carrinho.AssociarCarrinho(carrinhoSessao.CarrinhoId);

        return carrinho;
    }
}