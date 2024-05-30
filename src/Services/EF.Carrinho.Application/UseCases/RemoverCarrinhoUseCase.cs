using EF.Carrinho.Application.UseCases.Interfaces;
using EF.Carrinho.Domain.Repository;
using EF.Core.Commons.UseCases;

namespace EF.Carrinho.Application.UseCases;

public class RemoverCarrinhoUseCase : CommonUseCase, IRemoverCarrinhoUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;

    public RemoverCarrinhoUseCase(ICarrinhoRepository carrinhoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
    }

    public async Task RemoverCarrinho(Guid carrinhoId)
    {
        var carrinho = await _carrinhoRepository.ObterPorId(carrinhoId);

        if (carrinho is not null)
        {
            _carrinhoRepository.Remover(carrinho);
            await PersistData(_carrinhoRepository.UnitOfWork);
        }
    }

    public async Task RemoverCarrinhoPorClienteId(Guid clienteId)
    {
        var carrinho = await _carrinhoRepository.ObterPorClienteId(clienteId);

        if (carrinho is not null)
        {
            _carrinhoRepository.Remover(carrinho);
            await PersistData(_carrinhoRepository.UnitOfWork);
        }
    }
}