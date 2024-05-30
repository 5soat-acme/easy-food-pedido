using EF.Cupons.Application.DTOs.Responses;

namespace EF.Cupons.Application.UseCases.Interfaces;

public interface IConsultarCupomUseCase
{
    Task<CupomDto?> ObterCupom(string codigoCupom, CancellationToken cancellationToken = default);
}