using System.ComponentModel.DataAnnotations;
using EF.Core.Commons.Communication;
using EF.Core.Commons.UseCases;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Repository;

namespace EF.Produtos.Application.UseCases;

public class RemoverProdutoUseCase : CommonUseCase, IRemoverProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public RemoverProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<OperationResult> Handle(Guid id)
    {
        var produto = await _produtoRepository.BuscarPorId(id);
        if (produto is null) throw new ValidationException("Produto não existe");
        _produtoRepository.Remover(produto!);
        await PersistData(_produtoRepository.UnitOfWork);
        return OperationResult.Success();
    }
}