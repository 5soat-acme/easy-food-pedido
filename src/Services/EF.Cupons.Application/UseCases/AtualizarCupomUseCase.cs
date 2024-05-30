using EF.Core.Commons.Communication;
using EF.Cupons.Application.DTOs.Requests;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;

namespace EF.Cupons.Application.UseCases;

public class AtualizarCupomUseCase : CupomCommonUseCase, IAtualizarCupomUseCase
{
    public AtualizarCupomUseCase(ICupomRepository cupomRepository) : base(cupomRepository)
    {
    }

    public async Task<OperationResult> Handle(AtualizarCupomDto dto)
    {
        if (!await ValidarCupom(dto.CupomId)) return OperationResult.Failure(ValidationResult);

        var cupom = await GetCupom(dto);
        if (!await ValidarOutroCupomVigente(cupom!)) return OperationResult.Failure(ValidationResult);

        _cupomRepository.Atualizar(cupom!);
        ValidationResult = await PersistData(_cupomRepository.UnitOfWork);

        if (!ValidationResult.IsValid) return OperationResult.Failure(ValidationResult);

        return OperationResult.Success();
    }

    private async Task<Cupom?> GetCupom(AtualizarCupomDto dto)
    {
        var cupomExistente = await _cupomRepository.Buscar(dto.CupomId);
        cupomExistente!.AlterarCodigoCupom(dto.CodigoCupom);
        cupomExistente.AlterarPorcentagemDesconto(dto.PorcentagemDesconto);
        cupomExistente.AlterarDatas(dto.DataInicio, dto.DataFim);
        return cupomExistente;
    }
}