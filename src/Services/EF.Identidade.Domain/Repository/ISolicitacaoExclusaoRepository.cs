using EF.Core.Commons.Repository;
using EF.Identidade.Domain.Models;

namespace EF.Identidade.Domain.Repository;

public interface ISolicitacaoExclusaoRepository : IRepository<SolicitacaoExclusao>
{
    Task<SolicitacaoExclusao?> BuscarPorClienteId(Guid clienteId);
    void Criar(SolicitacaoExclusao solicitacaoExclusao);
}