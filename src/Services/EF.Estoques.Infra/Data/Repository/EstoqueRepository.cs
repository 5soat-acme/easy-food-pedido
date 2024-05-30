using EF.Core.Commons.Repository;
using EF.Estoques.Domain.Models;
using EF.Estoques.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Estoques.Infra.Data.Repository;

public sealed class EstoqueRepository : IEstoqueRepository
{
    private readonly EstoqueDbContext _dbContext;

    public EstoqueRepository(EstoqueDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task<Estoque?> Buscar(Guid produtoId, CancellationToken cancellationToken)
    {
        return await _dbContext.Estoques.FirstOrDefaultAsync(x => x.ProdutoId == produtoId, cancellationToken);
    }

    public async Task<Estoque> Salvar(Estoque estoque, CancellationToken cancellationToken)
    {
        var estoqueExistente = _dbContext.Estoques
            .Where(p => p.Id == estoque.Id)
            .AsNoTracking()
            .SingleOrDefault();

        if (estoqueExistente is null)
        {
            // Inserir
            await _dbContext.Estoques.AddAsync(estoque, cancellationToken);
        }
        else
        {
            // Atualizar
            foreach (var mov in estoque.Movimentacoes) _dbContext.Entry(mov).State = EntityState.Added;
            _dbContext.Update(estoque);
        }

        return estoque;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}