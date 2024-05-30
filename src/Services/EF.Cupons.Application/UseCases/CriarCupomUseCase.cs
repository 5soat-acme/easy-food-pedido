using EF.Core.Commons.Communication;
using EF.Cupons.Application.DTOs.Requests;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;

namespace EF.Cupons.Application.UseCases;

public class CriarCupomUseCase : CupomCommonUseCase, ICriarCupomUseCase
{
    public CriarCupomUseCase(ICupomRepository cupomRepository) : base(cupomRepository)
    {
    }

    public async Task<OperationResult<Guid>> Handle(CriarCupomDto dto)
    {
        var cupom = GetCupom(dto);
        if (!await ValidarOutroCupomVigente(cupom)) return OperationResult<Guid>.Failure(ValidationResult);
        await _cupomRepository.Criar(cupom);
        ValidationResult = await PersistData(_cupomRepository.UnitOfWork);
        if (!ValidationResult.IsValid) return OperationResult<Guid>.Failure(ValidationResult);
        return OperationResult<Guid>.Success(cupom.Id);
    }

    private Cupom GetCupom(CriarCupomDto dto)
    {
        var cupom = new Cupom(dto.DataInicio, dto.DataFim, dto.CodigoCupom, dto.PorcentagemDesconto,
            CupomStatus.Ativo);
        foreach (var prod in dto.Produtos) cupom.AdicionarProduto(new CupomProduto(cupom.Id, prod.ProdutoId));
        return cupom;
    }
}