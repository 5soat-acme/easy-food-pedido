using EF.Cupons.Application.UseCases.Interfaces;
using EF.Pedidos.Application.DTOs.Gateways;
using EF.Pedidos.Application.Gateways;

namespace EF.Pedidos.Infra.Adapters.Cupons;

public class CupomAdapter : ICupomService
{
    private readonly IConsultarCupomUseCase _consultarCupomUseCase;

    public CupomAdapter(IConsultarCupomUseCase consultarCupomUseCase)
    {
        _consultarCupomUseCase = consultarCupomUseCase;
    }

    public async Task<CupomDto?> ObterCupomPorCodigo(string codigo)
    {
        var cupom = await _consultarCupomUseCase.ObterCupom(codigo);
        return CumpomToDomainMapper.Map(cupom);
    }
}