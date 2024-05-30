using EF.Core.Commons.Communication;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;

namespace EF.Cupons.Application.UseCases;

public class InativarCupomUseCase : CupomCommonUseCase, IInativarCupomUseCase
{
    public InativarCupomUseCase(ICupomRepository cupomRepository) : base(cupomRepository)
    {
    }

    public async Task<OperationResult> Handle(Guid id)
    {
        if (!await ValidarCupom(id, false))
            return OperationResult.Failure(ValidationResult);

        var cupom = await GetCupom(id);
        _cupomRepository.Atualizar(cupom!);
        ValidationResult = await PersistData(_cupomRepository.UnitOfWork);
        if (!ValidationResult.IsValid) return OperationResult.Failure(ValidationResult);
        return OperationResult.Success();
    }

    private async Task<Cupom> GetCupom(Guid id)
    {
        var cupomExistente = await _cupomRepository.Buscar(id);
        cupomExistente!.InativarCupom();
        return cupomExistente;
    }
}