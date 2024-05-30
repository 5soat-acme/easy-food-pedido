using EF.Carrinho.Application.DTOs.Requests;
using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Domain.Repository;
using EF.Core.Commons.Communication;
using EF.Core.Commons.UseCases;

namespace EF.Carrinho.Application.UseCases;

public class RemoverItemCarrinhoUseCase : CommonUseCase, IRemoverItemCarrinhoUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IConsultarCarrinhoUseCase _consultarCarrinhoUseCase;

    public RemoverItemCarrinhoUseCase(ICarrinhoRepository carrinhoRepository,
        IConsultarCarrinhoUseCase consultarCarrinhoUseCase)
    {
        _carrinhoRepository = carrinhoRepository;
        _consultarCarrinhoUseCase = consultarCarrinhoUseCase;
    }

    public async Task<OperationResult> RemoverItemCarrinho(Guid itemId, CarrinhoSessaoDto carrinhoSessao)
    {
        var carrinho = await _consultarCarrinhoUseCase.ObterCarrinho(carrinhoSessao);

        if (carrinho is null) return OperationResult.Failure("Carrinho não encontrado");

        var item = carrinho.Itens.FirstOrDefault(f => f.Id == itemId);

        if (item is null) return OperationResult.Success();

        carrinho.RemoverItem(item);
        _carrinhoRepository.RemoverItem(item);

        if (!carrinho.Itens.Any())
            _carrinhoRepository.Remover(carrinho);
        else
            _carrinhoRepository.Atualizar(carrinho);

        await PersistData(_carrinhoRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult.Failure(ValidationResult);

        return OperationResult.Success();
    }
}