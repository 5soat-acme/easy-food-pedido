using EF.Core.Commons.Communication;
using EF.Cupons.Application.DTOs.Requests;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;

namespace EF.Cupons.Application.UseCases;

public class InserirProdutoUseCase : CupomCommonUseCase, IInserirProdutoUseCase
{
    public InserirProdutoUseCase(ICupomRepository cupomRepository) : base(cupomRepository)
    {
    }

    public async Task<OperationResult> Handle(InserirProdutoDto dto)
    {
        if (!await ValidarCupom(dto.CupomId)) return OperationResult.Failure(ValidationResult);

        var produtos = await GetProdutos(dto);
        if (produtos.Count > 0)
        {
            await _cupomRepository.InserirProdutos(produtos);
            await PersistData(_cupomRepository.UnitOfWork);
        }

        return OperationResult.Success();
    }

    private async Task<IList<CupomProduto>> GetProdutos(InserirProdutoDto dto)
    {
        var produtos = new List<CupomProduto>();
        foreach (var p in dto.Produtos)
        {
            var prodExiste =
                await _cupomRepository.BuscarCupomProduto(dto.CupomId, p) is not null;
            if (!prodExiste)
                produtos.Add(new CupomProduto(dto.CupomId, p));
        }

        return produtos;
    }
}