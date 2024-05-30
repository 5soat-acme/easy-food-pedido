using EF.Cupons.Application.DTOs.Responses;
using EF.Cupons.Application.Mappings;
using EF.Cupons.Application.UseCases.Interfaces;
using EF.Cupons.Domain.Repository;

namespace EF.Cupons.Application.UseCases;

public class ConsultarCupomUseCase : IConsultarCupomUseCase
{
    private readonly ICupomRepository _cupomRepository;

    public ConsultarCupomUseCase(ICupomRepository cupomRepository)
    {
        _cupomRepository = cupomRepository;
    }

    public async Task<CupomDto?> ObterCupom(string codigoCupom, CancellationToken cancellationToken = default)
    {
        var cupom = await _cupomRepository.BuscarCupomVigente(codigoCupom, cancellationToken);
        return CupomDomainToDtoMapper.Map(cupom);
    }
}