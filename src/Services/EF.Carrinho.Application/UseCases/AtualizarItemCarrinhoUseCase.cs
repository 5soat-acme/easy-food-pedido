using System.Data;
using EF.Carrinho.Application.DTOs.Requests;
using EF.Carrinho.Application.Gateways;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Domain.Repository;
using EF.Core.Commons.Communication;

namespace EF.Carrinho.Application.UseCases;

public class AtualizarItemCarrinhoUseCase : CarrinhoCommonUseCase, IAtualizarItemCarrinhoUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IConsultarCarrinhoUseCase _consultarCarrinhoUseCase;

    public AtualizarItemCarrinhoUseCase(ICarrinhoRepository carrinhoRepository, IEstoqueService estoqueService,
        IConsultarCarrinhoUseCase consultarCarrinhoUseCase) : base(estoqueService)
    {
        _carrinhoRepository = carrinhoRepository;
        _consultarCarrinhoUseCase = consultarCarrinhoUseCase;
    }

    public async Task<OperationResult> AtualizarItem(AtualizarItemDto itemDto, CarrinhoSessaoDto carrinhoSessao)
    {
        var carrinho = await _consultarCarrinhoUseCase.ObterCarrinho(carrinhoSessao);

        if (carrinho is null) return OperationResult.Failure("O carrinho está vazio");

        carrinho.AtualizarQuantidadeItem(itemDto.ItemId, itemDto.Quantidade);
        var item = carrinho.ObterItemPorId(itemDto.ItemId);

        if (item is null) throw new NoNullAllowedException("Item não existe");

        if (!await ValidarEstoque(item!)) return OperationResult.Failure("Produto sem estoque");

        _carrinhoRepository.Atualizar(carrinho);

        await PersistData(_carrinhoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult.Failure(ValidationResult);

        return OperationResult.Success();
    }
}