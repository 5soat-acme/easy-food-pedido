using EF.Core.Commons.Communication;
using EF.Cupons.Application.DTOs.Requests;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;

namespace EF.Cupons.Application.UseCases;

public class RemoverProdutoUseCase : CupomCommonUseCase, IRemoverProdutoUseCase
{
    public RemoverProdutoUseCase(ICupomRepository cupomRepository) : base(cupomRepository)
    {
    }

    public async Task<OperationResult> Handle(RemoverProdutoDto dto)
    {
        if (!await ValidarCupom(dto.CupomId)) return OperationResult.Failure(ValidationResult);

        var produtos = await GetProdutos(dto);
        if (produtos.Count > 0)
        {
            _cupomRepository.RemoverProdutos(produtos);
            await PersistData(_cupomRepository.UnitOfWork);
        }

        return OperationResult.Success();
    }

    private async Task<IList<CupomProduto>> GetProdutos(RemoverProdutoDto dto)
    {
        var produtos = new List<CupomProduto>();
        foreach (var p in dto.Produtos)
        {
            var cupomProd = await _cupomRepository.BuscarCupomProduto(dto.CupomId, p);
            if (cupomProd is not null)
                produtos.Add(cupomProd);
        }

        return produtos;
    }
}