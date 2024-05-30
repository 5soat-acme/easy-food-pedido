using EF.Carrinho.Domain.Models;
using EF.Carrinho.Domain.Repository;
using EF.Core.Commons.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Carrinho.Infra.Data.Repository;

public sealed class CarrinhoRepository : ICarrinhoRepository
{
    private readonly CarrinhoDbContext _context;

    public CarrinhoRepository(CarrinhoDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<CarrinhoCliente?> ObterPorClienteId(Guid clienteId)
    {
        return await _context.Carrinhos
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
    }

    public async Task<CarrinhoCliente?> ObterPorId(Guid id)
    {
        return await _context.Carrinhos
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public void Criar(CarrinhoCliente carrinho)
    {
        _context.Carrinhos.Add(carrinho);
    }

    public void Atualizar(CarrinhoCliente carrinho)
    {
        _context.Carrinhos.Update(carrinho);
    }

    public void Remover(CarrinhoCliente carrinho)
    {
        _context.Carrinhos.Remove(carrinho);
    }

    public void AdicionarItem(Item item)
    {
        _context.Itens.Add(item);
    }

    public void AtualizarItem(Item item)
    {
        _context.Itens.Update(item);
    }

    public void RemoverItem(Item item)
    {
        _context.Itens.Remove(item);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}