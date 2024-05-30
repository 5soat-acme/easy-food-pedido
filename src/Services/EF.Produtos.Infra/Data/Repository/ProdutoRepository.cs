using EF.Core.Commons.Repository;
using EF.Produtos.Domain.Models;
using EF.Produtos.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Produtos.Infra.Data.Repository;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly ProdutoDbContext _dbContext;

    public ProdutoRepository(ProdutoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public void Criar(Produto produto)
    {
        _dbContext.Produtos.AddAsync(produto);
    }

    public void Atualizar(Produto produto)
    {
        _dbContext.Produtos.Update(produto);
    }

    public async Task<IEnumerable<Produto>> Buscar(ProdutoCategoria? categoria)
    {
        return await _dbContext.Produtos
            .Where(produto => produto.Ativo && (categoria == null || produto.Categoria == categoria))
            .ToListAsync();
    }

    public async Task<Produto?> BuscarPorId(Guid produtoId)
    {
        return await _dbContext.Produtos.FirstOrDefaultAsync(produto => produto.Id == produtoId);
    }

    public void Remover(Produto produto)
    {
        _dbContext.Produtos.Remove(produto);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}