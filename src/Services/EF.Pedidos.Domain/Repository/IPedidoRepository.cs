using EF.Core.Commons.Repository;
using EF.Pedidos.Domain.Models;

namespace EF.Pedidos.Domain.Repository;

public interface IPedidoRepository : IRepository<Pedido>
{
    Task<Pedido> ObterPorId(Guid id);
    void Criar(Pedido pedido);
    void Atualizar(Pedido pedido);
}