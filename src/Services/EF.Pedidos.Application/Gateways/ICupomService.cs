using EF.Pedidos.Application.DTOs.Gateways;

namespace EF.Pedidos.Application.Gateways;

public interface ICupomService
{
    Task<CupomDto?> ObterCupomPorCodigo(string codigo);
}