using EF.Core.Commons.UseCases;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;

namespace EF.Cupons.Application.UseCases;

public abstract class CupomCommonUseCase : CommonUseCase
{
    protected readonly ICupomRepository _cupomRepository;

    public CupomCommonUseCase(ICupomRepository cupomRepository)
    {
        _cupomRepository = cupomRepository;
    }

    protected async Task<bool> ValidarCupom(Guid cupomId, bool validarVigencia = true)
    {
        var cupom = await _cupomRepository.Buscar(cupomId);
        if (cupom is null)
        {
            AddError("Cupom não encontrado");
            return false;
        }

        if (validarVigencia && cupom.DataInicio <= DateTime.Now.Date)
        {
            AddError("Não é possível alterar um cupom em vigência");
            return false;
        }

        return true;
    }

    protected async Task<bool> ValidarOutroCupomVigente(Cupom cupom, CancellationToken cancellationToken = default)
    {
        var listaCupom = await _cupomRepository.BuscarCupomVigenteEmPeriodo(cupom.CodigoCupom, cupom.DataInicio,
            cupom.DataFim, cancellationToken);
        listaCupom = listaCupom.Where(x => x.Id != cupom.Id).ToList();

        if (listaCupom.Count > 0)
        {
            AddError("Já existe cupom com este código em vigência para o mesmo período");
            return false;
        }

        return true;
    }
}