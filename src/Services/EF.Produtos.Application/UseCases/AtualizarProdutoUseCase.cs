using System.ComponentModel.DataAnnotations;
using EF.Core.Commons.Communication;
using EF.Core.Commons.UseCases;
using EF.Produtos.Application.DTOs.Requests;
using EF.Produtos.Application.UseCases.Interfaces;
using EF.Produtos.Domain.Repository;

namespace EF.Produtos.Application.UseCases;

public class AtualizarProdutoUseCase : CommonUseCase, IAtualizarProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public AtualizarProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<OperationResult<Guid>> Handle(AtualizarProdutoDto dto)
    {
        var produto = await _produtoRepository.BuscarPorId(dto.ProdutoId);
        if (produto is null) throw new ValidationException("Produto não existe");
        produto.AlterarProduto(dto.Nome, dto.ValorUnitario, dto.Categoria, dto.Descricao,
            dto.TempoPreparoEstimado, dto.Ativo);
        _produtoRepository.Atualizar(produto);
        await PersistData(_produtoRepository.UnitOfWork);
        return OperationResult<Guid>.Success(produto.Id);
    }
}