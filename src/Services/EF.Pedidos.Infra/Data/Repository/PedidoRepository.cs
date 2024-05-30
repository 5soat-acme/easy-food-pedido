using EF.Core.Commons.Repository;
using EF.Pedidos.Domain.Models;
using EF.Pedidos.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Pedidos.Infra.Data.Repository;

public sealed class PedidoRepository : IPedidoRepository
{
    private readonly PedidoDbContext _context;

    public PedidoRepository(PedidoDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Pedido> ObterPorId(Guid id)
    {
        return await _context.Pedidos
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public void Criar(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
    }

    public void Atualizar(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}