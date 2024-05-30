using EF.Core.Commons.Repository;
using EF.Estoques.Domain.Models;

namespace EF.Estoques.Domain.Repository;

public interface IEstoqueRepository : IRepository<Estoque>
{
    Task<Estoque?> Buscar(Guid produtoId, CancellationToken cancellationToken = default);
    Task<Estoque> Salvar(Estoque estoque, CancellationToken cancellationToken = default);
}