using EF.Core.Commons.Communication;
using EF.Core.Commons.UseCases;
using EF.Produtos.Application.DTOs.Requests;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Models;
using EF.Produtos.Domain.Repository;

namespace EF.Produtos.Application.UseCases;

public class CriarProdutoUseCase : CommonUseCase, ICriarProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public CriarProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<OperationResult<Guid>> Handle(CriarProdutoDto dto)
    {
        var produto = new Produto(dto.Nome, dto.ValorUnitario, dto.Categoria, dto.TempoPreparoEstimado,
            dto.Descricao);
        _produtoRepository.Criar(produto);
        await PersistData(_produtoRepository.UnitOfWork);
        return OperationResult<Guid>.Success(produto.Id);
    }
}