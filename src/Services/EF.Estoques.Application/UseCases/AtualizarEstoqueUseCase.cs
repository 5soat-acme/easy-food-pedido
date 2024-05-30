using EF.Core.Commons.Communication;
using EF.Core.Commons.UseCases;
using EF.Estoques.Application.DTOs.Requests;
using EF.Estoques.Application.UseCases.Interfaces;
using EF.Estoques.Domain.Models;
using EF.Estoques.Domain.Repository;

namespace EF.Estoques.Application.UseCases;

public class AtualizarEstoqueUseCase : CommonUseCase, IAtualizarEstoqueUseCase
{
    private readonly IEstoqueRepository _estoqueRepository;

    public AtualizarEstoqueUseCase(IEstoqueRepository estoqueRepository)
    {
        _estoqueRepository = estoqueRepository;
    }

    public async Task<OperationResult<Guid>> Handle(AtualizarEstoqueDto dto)
    {
        var estoque = await GetEstoque(dto);
        estoque.AdicionarMovimentacao(new MovimentacaoEstoque(estoque.Id, dto.ProdutoId, dto.Quantidade,
            dto.TipoMovimentacao,
            dto.OrigemMovimentacao, DateTime.Now));
        await _estoqueRepository.Salvar(estoque);
        await PersistData(_estoqueRepository.UnitOfWork);
        if (!ValidationResult.IsValid) return OperationResult<Guid>.Failure(ValidationResult);
        return OperationResult<Guid>.Success(estoque.Id);
    }

    private async Task<Estoque> GetEstoque(AtualizarEstoqueDto dto)
    {
        var estoqueExistente = await _estoqueRepository.Buscar(dto.ProdutoId);
        var estoque = estoqueExistente ?? new Estoque(dto.ProdutoId, 0);
        estoque.AtualizarQuantidade(dto.Quantidade, dto.TipoMovimentacao);
        return estoque;
    }
}