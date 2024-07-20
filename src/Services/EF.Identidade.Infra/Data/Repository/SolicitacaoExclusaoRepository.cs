using EF.Core.Commons.Repository;
using EF.Identidade.Domain.Models;
using EF.Identidade.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace EF.Identidade.Infra.Data.Repository;

public class SolicitacaoExclusaoRepository : ISolicitacaoExclusaoRepository
{
    private readonly IdentidadeDbContext _dbContext;

    public SolicitacaoExclusaoRepository(IdentidadeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task<SolicitacaoExclusao?> BuscarPorClienteId(Guid clienteId)
    {
        return await _dbContext.SolicitacaoExclusao.FirstOrDefaultAsync(x => x.ClienteId == clienteId);
    }

    public void Criar(SolicitacaoExclusao solicitacaoExclusao)
    {
        _dbContext.SolicitacaoExclusao.AddAsync(solicitacaoExclusao);
    }
}