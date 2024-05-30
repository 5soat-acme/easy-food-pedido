using EF.Core.Commons.Repository;
using EF.Cupons.Domain.Models;
using EF.Cupons.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Cupons.Infra.Data.Repository;

public sealed class CupomRepository : ICupomRepository
{
    private readonly CupomDbContext _dbContext;

    public CupomRepository(CupomDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task<Cupom?> Buscar(Guid cupomId, CancellationToken cancellationToken)
    {
        return await _dbContext.Cupons
            .Include(x => x.CupomProdutos)
            .Where(x => x.Id == cupomId && x.Status == CupomStatus.Ativo)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CupomProduto?> BuscarCupomProduto(Guid cupomId, Guid produtoId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.CupomProdutos
            .Where(x => x.CupomId == cupomId
                        && x.ProdutoId == produtoId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Cupom?> BuscarCupomVigente(string codigoCupom, CancellationToken cancellationToken)
    {
        var dataAtual = DateTime.Now.ToUniversalTime();
        return await _dbContext.Cupons.Include(x => x.CupomProdutos)
            .Where(x => x.CodigoCupom == codigoCupom
                        && x.Status == CupomStatus.Ativo
                        && x.DataInicio <= dataAtual
                        && x.DataFim >= dataAtual)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<Cupom>> BuscarCupomVigenteEmPeriodo(string codigoCupom, DateTime dataInicio,
        DateTime dataFim, CancellationToken cancellationToken)
    {
        return await _dbContext.Cupons
            .Where(x => x.CodigoCupom == codigoCupom
                        && x.Status == CupomStatus.Ativo
                        && x.DataInicio <= dataFim
                        && x.DataFim >= dataInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<Cupom> Criar(Cupom cupom, CancellationToken cancellationToken)
    {
        await _dbContext.Cupons.AddAsync(cupom, cancellationToken);
        return cupom;
    }

    public Cupom Atualizar(Cupom cupom, CancellationToken cancellationToken)
    {
        _dbContext.Cupons.Update(cupom);
        return cupom;
    }

    public async Task InserirProdutos(IList<CupomProduto> produtos, CancellationToken cancellationToken)
    {
        await _dbContext.CupomProdutos.AddRangeAsync(produtos, cancellationToken);
    }

    public void RemoverProdutos(IList<CupomProduto> produtos, CancellationToken cancellationToken)
    {
        _dbContext.CupomProdutos.RemoveRange(produtos);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}