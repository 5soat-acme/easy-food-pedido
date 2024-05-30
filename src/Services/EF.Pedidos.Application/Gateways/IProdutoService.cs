using EF.Pedidos.Application.DTOs.Gateways;

namespace EF.Pedidos.Application.Gateways;

public interface IProdutoService
{
    Task<ProdutoDto?> ObterPorId(Guid id);
}